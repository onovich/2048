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
    public Dictionary<Vector2Int, IBlockEntity> BlocksInMap = new Dictionary<Vector2Int, IBlockEntity>();
    public List<IBlockEntity> BlocksToRefresh = new List<IBlockEntity>();
    public List<Vector2Int> BlocksToRemove = new List<Vector2Int>();

    //public GameObject sample;


    private float ox = -2.06f;
    private float oy = -2.07f;
    private float blockSize = 1.37f;

    public Sprite[] blockSprites = new Sprite[11];

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
        //Debug.Log("执行生成");
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

            BlocksInMap.Add(Passable[index], blockEntityComponent);
            Passable.Remove(Passable[index]);


        }
        else
        {
            //Debug.Log("世界被填满");
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

    public void RemoveList()
    {
        foreach(Vector2Int blockEntity in BlocksToRemove)
        {
            BlocksInMap[blockEntity].Remove();
            BlocksInMap.Remove(blockEntity);
            BlocksToRemove.Remove(blockEntity);
        }
    }


    public void RefreshTagDisplay()
    {
        foreach (IBlockEntity blockEntity in BlocksInMap.Values)
        {
            blockEntity.RefreshSprite(blockSprites);

        }
        Debug.Log("已更新点数显示");
    }

    IEnumerator WorldMoving()
    {
        float timeCost = 0;
        while (timeCost < .1f)
        {
            foreach (BlockEntity blockEntity in BlocksToRefresh)
            {
                if (blockEntity != null && blockEntity.target != null)
                {
                    Vector2 blockVelocity = Vector2.zero;
                    Vector2 pos = blockEntity.transform.position;
                   
                    Vector2 target = new Vector2(ox + blockSize * blockEntity.target.Value.x, oy + blockSize * blockEntity.target.Value.y);

                    //blockEntity.transform.position = Vector2.SmoothDamp(pos, target, ref blockVelocity, .1f);
                    blockEntity.transform.position = Vector2.Lerp(pos, target, timeCost / .1f);
                    pos = blockEntity.transform.position;
                }
                

            }
            timeCost += .002f;
            yield return new WaitForSecondsRealtime(.002f);
            
        }
        
        foreach (BlockEntity blockEntity in BlocksToRefresh)
        {
            if (blockEntity != null && blockEntity.target != null)
            {
                Vector2 target = new Vector2(ox + blockSize * blockEntity.target.Value.x, oy + blockSize * blockEntity.target.Value.y);
                blockEntity.transform.position = target;
            }
            
        }

        //RemoveList();
        BlocksToRefresh.Clear();
        RefreshTagDisplay();

        OnWorldMoveDoneEvent?.Invoke();
        //Debug.Log("MoveDone");

        

    }


    bool OutOfMap(int x,int y)
    {
        if (x > 3 || x < 0 || y > 3 || y < 0)
            return true;
        else
            return false;
    }
    void SetTarget(int selfX,int selfY,int targetX,int targetY)
    {
        if(selfX!=targetX || selfY != targetY)
        {
            Vector2Int pos = new Vector2Int(selfX, selfY);
            Vector2Int target = new Vector2Int(targetX, targetY);

            IBlockEntity entity = BlocksInMap[pos];

            entity.target = target;
            BlocksInMap.Add(target, entity);
            BlocksInMap.Remove(pos);
            Passable.Add(pos);
            Passable.Remove(target);

            BlocksToRefresh.Add(entity);
        }

        
       
    }
    bool HasBlock(int x,int y)
    {
        //return !Passable.Contains(new Vector2Int(x, y));
        return BlocksInMap.ContainsKey(new Vector2Int(x,y));
    }

    bool TagEualToSelf(int selfX,int selfY,int targetX,int targetY)
    {
        Vector2Int pos = new Vector2Int(selfX, selfY);
        Vector2Int target = new Vector2Int(targetX, targetY);
        return BlocksInMap[pos].blockTag == BlocksInMap[target].blockTag;
    }

    void Combine(int selfX, int selfY, int targetX, int targetY)
    {
        Debug.Log("合并判定预备:"+selfX+","+selfY+";"+targetX+","+targetY);
        if (selfX != targetX || selfY != targetY)
        {
            Debug.Log("合并判定");
            Vector2Int pos = new Vector2Int(selfX, selfY);
            Vector2Int target = new Vector2Int(targetX, targetY);

            IBlockEntity entity = BlocksInMap[pos];
            //IBlockEntity targetEntity = BlocksInMap[target];

            BlocksToRefresh.Remove(entity);
            BlocksInMap.Remove(pos);
            entity.Remove();
            Passable.Add(pos);
            BlocksInMap[target].blockTag *= 2;
            
            //BlocksToRefresh.Add(targetEntity);
        }
    }

    void CombineTest(int dir)
    {
        if (dir == 1)
        {
            for (int y = 2; y >= 0; y--)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (HasBlock(x, y))
                    {
                        if (HasBlock(x, y+1) && TagEualToSelf(x, y, x, y + 1))
                        {
                            Combine(x, y, x, y+1);
                            //break;
                        }
                    }

                }
            }
        }
        if (dir == 2)
        {
            for (int y = 1; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (HasBlock(x, y))
                    {
                        if (HasBlock(x, y - 1) && TagEualToSelf(x, y, x, y - 1))
                        {
                            Combine(x, y, x, y - 1);
                            //break;
                        }
                    }

                }
            }
        }
        if (dir == 3)
        {
            for (int x = 1; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (HasBlock(x, y))
                    {
                        if (HasBlock(x - 1, y) && TagEualToSelf(x, y, x - 1, y))
                        {
                            Combine(x, y, x - 1, y);
                            //break;
                        }
                    }

                }
            }
        }
        if (dir == 4)
        {
            for (int x = 2; x >= 0; x--)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (HasBlock(x, y))
                    {
                        if (HasBlock(x + 1, y) && TagEualToSelf(x, y, x + 1, y))
                        {
                            Combine(x, y, x + 1, y);
                            //break;
                        }
                    }

                }
            }
        }

    }









    void MoveTest(int dir)
    {
        if (dir == 1)
        {
            for (int y = 2; y >=0; y--)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (HasBlock(x, y))
                    {
                        for (int j = y; j < 4; j++)
                        {
                            if ((OutOfMap(x, j + 1) || (HasBlock(x, j + 1))))
                            {
                                SetTarget(x, y, x, j );
                                break;
                            }
                        }
                    }
                    
                }
            }
        }
        if (dir == 2)
        {
            for (int y = 1; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (HasBlock(x,y))
                    {
                        for (int j = y; j >= 0; j--)
                        {
                            if ((OutOfMap(x, j - 1) || (HasBlock(x, j - 1))))
                            {
                                SetTarget(x, y, x, j );
                                break;
                            }
                        }
                    }
                    
                }
            }
        }
        if (dir == 3)
        {
            for(int x = 1; x < 4; x++)
            {
                for(int y = 0; y < 4; y++)
                {
                    if (HasBlock(x, y))
                    {
                        for (int i = x; i >= 0; i--)
                        {
                            if ((OutOfMap(i - 1, y) || (HasBlock(i - 1, y))))
                            {
                                SetTarget(x, y, i, y);
                                break;
                            }
                        }
                    }
                    
                }
            }
        }
        if (dir == 4)
        {
            for (int x = 2; x >=0; x--)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (HasBlock(x, y))
                    {
                        for (int i = x; i < 4; i++)
                        {
                            if ((OutOfMap(i + 1, y) || (HasBlock(i + 1, y))))
                            {
                                SetTarget(x, y, i, y);
                                break;
                            }
                        }
                    }
                    
                }
            }
        }

    }




    public void WorldMove(int dir)
    {
        //CombineTest(dir);
        MoveTest(dir);
        CombineTest(dir);
        MoveTest(dir);

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
