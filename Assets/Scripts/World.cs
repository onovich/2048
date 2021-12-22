using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IWorld
{
    void Generate();
    void WorldAction(object sender, MoveEventArgs e);
    void WorldMove(int dir);
    bool IfMovable();
    void End();
    void RefreshDisplay();
    event OnEndEventHandler OnEndEvent;
    event OnWorldMoveDoneEventHandler OnWorldMoveDoneEvent;
    event OnMoveCalculateDoneEventHandler OnMoveCalculateDoneEvent;
}




public delegate void OnEndEventHandler();
public delegate void OnMoveCalculateDoneEventHandler();
public delegate void OnWorldMoveDoneEventHandler();


public class World : MonoBehaviour, IWorld
{
    public event OnEndEventHandler OnEndEvent;
    public event OnWorldMoveDoneEventHandler OnWorldMoveDoneEvent;
    public event OnMoveCalculateDoneEventHandler OnMoveCalculateDoneEvent;

    //public WorldMap[,] WorldMap = new WorldMap[4,4];
    public List<Vector2Int> Passable = new List<Vector2Int>();
    public Dictionary<Vector2Int, IBlockEntity> Blocks = new Dictionary<Vector2Int, IBlockEntity>();
    public List<IBlockEntity> BlocksToRefresh = new List<IBlockEntity>();

    //public GameObject sample;


    private float ox = -2.06f;
    private float oy = -2.07f;
    private float blockSize = 1.37f;

    private Sprite[] blockSprites = new Sprite[11];

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Passable.Add(new Vector2Int(i, j));
            }
        }
        for (int i = 1; i < 12; i++)
        {
            blockSprites[i - 1] = Resources.Load<Sprite>("" + Mathf.Pow(2, i));
        }
        Generate();
    }


    public void Generate()
    {
        Debug.Log("执行生成");
        if (Passable.Count > 0)
        {

            int index = Random.Range(0, Passable.Count);
            int n = Random.Range(0, 2);
            //WorldMap[Passable[index].x, Passable[index].y].tag = n == 0 ? 2 : 4;
            float worldX = ox + blockSize * Passable[index].x;
            float worldY = oy + blockSize * Passable[index].y;


            GameObject prefab = Resources.Load<GameObject>("block0") as GameObject;
            GameObject blockEntity = Instantiate(prefab, transform);
            BlockEntity blockEntityComponent = blockEntity.GetComponent<BlockEntity>();

            blockEntityComponent.blockTag = n == 0 ? 2 : 4;
            blockEntity.transform.position = new Vector3(worldX, worldY, 0);
            blockEntity.GetComponent<SpriteRenderer>().sprite = n == 0 ? blockSprites[0] : blockSprites[1];

            Blocks.Add(Passable[index], blockEntityComponent);
            Passable.Remove(Passable[index]);


        }
        else
        {
            Debug.Log("世界被填满");
            OnEndEvent?.Invoke();
        }

    }




    public void WorldAction(object sender, MoveEventArgs e)
    {
        if (e.dir != 0)
        {
            WorldMove(e.dir);
            //Debug.Log(WorldMap);
        }
    }

    public void RefreshDisplay()
    {
        StartCoroutine(WorldMoving());





    }

    

    IEnumerator WorldMoving()
    {
        float timeCost = 0;
        while (timeCost < .1f)
        {
            foreach (BlockEntity blockEntity in BlocksToRefresh)
            {
                Vector2 blockVelocity = Vector2.zero;
                Vector2 pos = blockEntity.transform.position;
                Vector2 target = new Vector2(ox + blockSize * blockEntity.target.Value.x, oy + blockSize * blockEntity.target.Value.y);

                //blockEntity.transform.position = Vector2.SmoothDamp(pos, target, ref blockVelocity, .1f);
                blockEntity.transform.position = Vector2.Lerp(pos, target, timeCost/.1f);
                pos = blockEntity.transform.position;

            }
            timeCost += .002f;
            yield return new WaitForSecondsRealtime(.002f);
            
        }
        foreach (BlockEntity blockEntity in BlocksToRefresh)
        {
            Vector2 target = new Vector2(ox + blockSize * blockEntity.target.Value.x, oy + blockSize * blockEntity.target.Value.y);
            blockEntity.transform.position = target;
        }

        BlocksToRefresh.Clear();


        OnWorldMoveDoneEvent?.Invoke();
        Debug.Log("MoveDone");

    }



    public void WorldMove(int dir)
    {

        if (dir == 1)
        {
            for (int y = 2; y >= 0; y--)//逐y,且忽略已顶格的第1列
            {
                for (int x = 0; x < 4; x++)//逐x,逐格
                {
                    if (Blocks.ContainsKey(new Vector2Int(x, y)))
                    {
                        //从自己向边缘，去逐格扫描
                        if (y + 1 < 4)
                        {
                            for (int j = y + 1; j < 4; j++)
                            {
                                //一旦发现非空格（在Blocks中有成员），
                                if (!Passable.Contains(new Vector2Int(x, j)))
                                {
                                    //如果非空格点数与自己相同，则增加自己的点数(但不break循环)
                                    if (Blocks[new Vector2Int(x, j)].blockTag == Blocks[new Vector2Int(x, y)].blockTag)
                                    {
                                        Blocks[new Vector2Int(x, y)].blockTag *= 2;
                                    }
                                    //如果非空格点数与自己不相同，则判定其向下一格为目标，且break当前循环
                                    else
                                    {
                                        //如果相邻则无动作，如果非相邻则需要动作
                                        if (y - j > 1)
                                        {
                                            //将当前对象添加到待更新列表
                                            BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                            //设定移动动画目标
                                            Blocks[new Vector2Int(x, y)].target = new Vector2Int(x, j - 1);
                                            //创建目标
                                            Blocks.Add(new Vector2Int(x, j - 1), Blocks[new Vector2Int(x, y)]);
                                            //修改目标坐标的点数
                                            Blocks[new Vector2Int(x, j - 1)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                            //移除当前坐标
                                            Blocks.Remove(new Vector2Int(x, y));
                                            //设置原坐标可行
                                            Passable.Add(new Vector2Int(x, y));
                                            //设置新坐标不可行
                                            Passable.Remove(new Vector2Int(x, j - 1));
                                        }
                                        //已经完成目标检定，退出循环
                                        break;
                                    }

                                    
                                }
                                if (j == 3)
                                {
                                    //一旦发现抵达边界，则判定边界为其目标
                                    //将当前对象添加到待更新列表
                                    /*
                                    if(BlocksToRefresh.Contains(Blocks[new Vector2Int(x, y)]))
                                    {
                                        Blocks[new Vector2Int(x, y)].Remove();
                                        BlocksToRefresh.Remove(Blocks[new Vector2Int(x, y)]);
                                    }
                                    */
                                    BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                    //设定移动动画目标
                                    Blocks[new Vector2Int(x, y)].target = new Vector2Int(x, 3);
                                    //创建目标

                                    if (Blocks.ContainsKey(new Vector2Int(x, 3)))
                                    {
                                        Blocks[new Vector2Int(x, 3)].Remove();
                                        Blocks.Remove(new Vector2Int(x, 3));
                                    }
                                    Blocks.Add(new Vector2Int(x, 3), Blocks[new Vector2Int(x, y)]);
                                    //修改目标坐标的点数
                                    Debug.Log("检测:x,j=" + x + "," + 3);
                                    Blocks[new Vector2Int(x, 3)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                    //移除当前坐标
                                    Blocks.Remove(new Vector2Int(x, y));
                                    //设置原坐标可行
                                    Passable.Add(new Vector2Int(x, y));
                                    //设置新坐标不可行
                                    Passable.Remove(new Vector2Int(x, 3));
                                    //已经完成目标检定，退出循环
                                    break;
                                }
                            }
                        } 
                    }

                }
            }
        }


        if (dir == 2)
        {
            for (int y = 1; y < 4; y++)//逐y,且忽略已顶格的第1列
            {
                for (int x = 0; x < 4; x++)//逐x,逐格
                {
                    if (Blocks.ContainsKey(new Vector2Int(x, y)))
                    {
                        //从自己向边缘，去逐格扫描
                        for (int j = y - 1; j >= 0; j--)
                        {
                            //一旦发现非空格（在Blocks中有成员），
                            if (!Passable.Contains(new Vector2Int(x, j)))
                            {
                                //如果非空格点数与自己相同，则增加自己的点数(但不break循环)
                                if (Blocks[new Vector2Int(x, j)].blockTag == Blocks[new Vector2Int(x, y)].blockTag)
                                {
                                    Blocks[new Vector2Int(x, y)].blockTag *= 2;
                                }
                                //如果非空格点数与自己不相同，则判定其向上一格为目标，且break当前循环
                                else
                                {
                                    //如果相邻则无动作，如果非相邻则需要动作
                                    if (y - j > 1)
                                    {
                                        //将当前对象添加到待更新列表
                                        BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                        //设定移动动画目标
                                        Blocks[new Vector2Int(x, y)].target = new Vector2Int(x, j + 1);
                                        //创建目标
                                        Blocks.Add(new Vector2Int(x, j + 1), Blocks[new Vector2Int(x, y)]);
                                        //修改目标坐标的点数
                                        Blocks[new Vector2Int(x, j + 1)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                        //移除当前坐标
                                        Blocks.Remove(new Vector2Int(x, y));
                                        //设置原坐标可行
                                        Passable.Add(new Vector2Int(x, y));
                                        //设置新坐标不可行
                                        Passable.Remove(new Vector2Int(x, j + 1));
                                    }
                                    //已经完成目标检定，退出循环
                                    break;
                                }

                                
                            }
                            if (j == 0)
                            {
                                //一旦发现抵达边界，则判定边界为其目标
                                //将当前对象添加到待更新列表
                                /*
                                if (BlocksToRefresh.Contains(Blocks[new Vector2Int(x, y)]))
                                {
                                    Blocks[new Vector2Int(x, y)].Remove();
                                    BlocksToRefresh.Remove(Blocks[new Vector2Int(x, y)]);
                                }
                                */
                                BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                //设定移动动画目标
                                Blocks[new Vector2Int(x, y)].target = new Vector2Int(x, 0);
                                //创建目标

                                if (Blocks.ContainsKey(new Vector2Int(x, 0)))
                                {
                                    Blocks[new Vector2Int(x, 0)].Remove();
                                    Blocks.Remove(new Vector2Int(x, 0));
                                }
                                Blocks.Add(new Vector2Int(x, 0), Blocks[new Vector2Int(x, y)]);
                                //修改目标坐标的点数
                                Debug.Log("检测:x,j=" + x + "," + 0);
                                Blocks[new Vector2Int(x, 0)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                //移除当前坐标
                                Blocks.Remove(new Vector2Int(x, y));
                                //设置原坐标可行
                                Passable.Add(new Vector2Int(x, y));
                                //设置新坐标不可行
                                Passable.Remove(new Vector2Int(x, 0));
                                //已经完成目标检定，退出循环
                                break;


                            }
                        }
                    }

                }
            }
        }


        if (dir == 3)
        {
            for (int x = 1; x < 4; x++)//逐x,且忽略已顶格的第1列
            {
                for (int y = 0; y < 4; y++)//逐y,逐格
                {
                    if (Blocks.ContainsKey(new Vector2Int(x, y)))
                    {
                        //从自己向边缘，去逐格扫描
                        for (int i = x - 1; i >= 0; i--)
                        {
                            //一旦发现非空格（在Blocks中有成员），
                            if (!Passable.Contains(new Vector2Int(i, y)))
                            {
                                //如果非空格点数与自己相同，则增加自己的点数(但不break循环)
                                if (Blocks[new Vector2Int(i, y)].blockTag == Blocks[new Vector2Int(x, y)].blockTag)
                                {
                                    Blocks[new Vector2Int(x, y)].blockTag *= 2;
                                }
                                //如果非空格点数与自己不相同，则判定其向右一格为目标，且break当前循环
                                else
                                {
                                    //如果相邻则无动作，如果非相邻则需要动作
                                    if (x - i > 1)
                                    {
                                        //将当前对象添加到待更新列表
                                        BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                        //设定移动动画目标
                                        Blocks[new Vector2Int(x, y)].target = new Vector2Int(i + 1, y);
                                        //创建目标
                                        Blocks.Add(new Vector2Int(i + 1, y), Blocks[new Vector2Int(x, y)]);
                                        //修改目标坐标的点数
                                        Blocks[new Vector2Int(i + 1, y)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                        //移除当前坐标
                                        Blocks.Remove(new Vector2Int(x, y));
                                        //设置原坐标可行
                                        Passable.Add(new Vector2Int(x, y));
                                        //设置新坐标不可行
                                        Passable.Remove(new Vector2Int(i + 1, y));
                                    }
                                    //已经完成目标检定，退出循环
                                    break;
                                }
                                
                            
                            }
                            if (i == 0)
                            {
                                //一旦发现抵达边界，则判定边界为其目标
                                //将当前对象添加到待更新列表
                                /*
                                if (BlocksToRefresh.Contains(Blocks[new Vector2Int(x, y)]))
                                {
                                    Blocks[new Vector2Int(x, y)].Remove();
                                    BlocksToRefresh.Remove(Blocks[new Vector2Int(x, y)]);
                                }
                                */
                                BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                //设定移动动画目标
                                Blocks[new Vector2Int(x, y)].target = new Vector2Int(0, y);
                                //创建目标
                                if (Blocks.ContainsKey(new Vector2Int(0,y)))
                                {
                                    Blocks[new Vector2Int(0,y)].Remove();
                                    Blocks.Remove(new Vector2Int(0,y));
                                }
                                Blocks.Add(new Vector2Int(0, y), Blocks[new Vector2Int(x, y)]);
                                //修改目标坐标的点数
                                Debug.Log("检测:i,y=" + 0 + "," + y);
                                Blocks[new Vector2Int(0, y)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                //移除当前坐标
                                Blocks.Remove(new Vector2Int(x, y));
                                //设置原坐标可行
                                Passable.Add(new Vector2Int(x, y));
                                //设置新坐标不可行
                                Passable.Remove(new Vector2Int(0, y));
                                //已经完成目标检定，退出循环
                                break;



                            }
                        }

                            
                    }

                }
            }
        }



        if (dir == 4)
        {
            for (int x = 2; x >= 0; x--)//逐x,且忽略已顶格的第1列
            {
                for (int y = 0; y < 4; y++)//逐y,逐格
                {
                    if (Blocks.ContainsKey(new Vector2Int(x, y)))
                    {
                        //从自己向边缘，去逐格扫描
                        if (x + 1 < 4)
                        {
                            for (int i = x + 1; i < 4; i++)
                            {
                                //一旦发现非空格（在Blocks中有成员），
                                if (!Passable.Contains(new Vector2Int(i, y)))
                                {
                                    //如果非空格点数与自己相同，则增加自己的点数(但不break循环)
                                    if (Blocks[new Vector2Int(i, y)].blockTag == Blocks[new Vector2Int(x, y)].blockTag)
                                    {
                                        Blocks[new Vector2Int(x, y)].blockTag *= 2;
                                    }
                                    //如果非空格点数与自己不相同，则判定其向左一格为目标，且break当前循环
                                    else
                                    {
                                        //如果相邻则无动作，如果非相邻则需要动作
                                        if (x - i > 1)
                                        {
                                            //将当前对象添加到待更新列表
                                            BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                            //设定移动动画目标
                                            Blocks[new Vector2Int(x, y)].target = new Vector2Int(i - 1, y);
                                            //创建目标
                                            Blocks.Add(new Vector2Int(i - 1, y), Blocks[new Vector2Int(x, y)]);
                                            //修改目标坐标的点数
                                            Blocks[new Vector2Int(i - 1, y)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                            //移除当前坐标
                                            Blocks.Remove(new Vector2Int(x, y));
                                            //设置原坐标可行
                                            Passable.Add(new Vector2Int(x, y));
                                            //设置新坐标不可行
                                            Passable.Remove(new Vector2Int(i - 1, y));
                                        }
                                        //已经完成目标检定，退出循环
                                        break;
                                    }
                                    
                                }
                                if (i == 3)
                                {
                                    //一旦发现抵达边界，则判定边界为其目标
                                    //将当前对象添加到待更新列表
                                    /*
                                    if (BlocksToRefresh.Contains(Blocks[new Vector2Int(x, y)]))
                                    {
                                        Blocks[new Vector2Int(x, y)].Remove();
                                        BlocksToRefresh.Remove(Blocks[new Vector2Int(x, y)]);
                                    }
                                    */
                                    BlocksToRefresh.Add(Blocks[new Vector2Int(x, y)]);
                                    //设定移动动画目标
                                    Blocks[new Vector2Int(x, y)].target = new Vector2Int(3, y);
                                    //创建目标
                                    if (Blocks.ContainsKey(new Vector2Int(3,y)))
                                    {
                                        Blocks[new Vector2Int(3,y)].Remove();
                                        Blocks.Remove(new Vector2Int(3,y));
                                    }
                                    Blocks.Add(new Vector2Int(3, y), Blocks[new Vector2Int(x, y)]);


                                    //修改目标坐标的点数
                                    Debug.Log("检测:i,y=" + 3 + "," + y);
                                    Blocks[new Vector2Int(3, y)].blockTag = Blocks[new Vector2Int(x, y)].blockTag;
                                    //移除当前坐标
                                    Blocks.Remove(new Vector2Int(x, y));
                                    //设置原坐标可行
                                    Passable.Add(new Vector2Int(x, y));
                                    //设置新坐标不可行
                                    Passable.Remove(new Vector2Int(3, y));
                                    //已经完成目标检定，退出循环
                                    break;
                                }
                            }
                        }
                        






                    }

                }
            }
        }




        OnMoveCalculateDoneEvent?.Invoke();
    }






    public bool IfMovable()
    {
        return false;
    }

    public void End()
    {
    }


}
