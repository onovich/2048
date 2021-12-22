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
    event OnEndEventHandler OnEndEvent;
    event OnWorldMoveDoneEventHandler OnWorldMoveDoneEvent;
}



public struct WorldMap
{
    public int tag;
    public int target;
    public Transform transform;
} 

public delegate void OnEndEventHandler();
public delegate void OnWorldMoveDoneEventHandler();


public class World : MonoBehaviour, IWorld
{
    public event OnEndEventHandler OnEndEvent;
    public event OnWorldMoveDoneEventHandler OnWorldMoveDoneEvent;

    public WorldMap[,] WorldMap = new WorldMap[4,4];


    public GameObject sample;
    

    private float ox = -2.06f;
    private float oy = -2.07f;
    private float blockSize = 1.37f;

    public void Generate()
    {
        int x = Random.Range(0, 3);
        int y = Random.Range(0, 3);
        int n = Random.Range(0, 1);
        if (n == 0)
        {
            WorldMap[x, y].tag = 2;
        }
        else
        {
            WorldMap[x, y].tag = 4;
        }

        float worldX = ox + blockSize * x;
        float worldY = oy + blockSize * y;

        GameObject g = Instantiate(sample, transform);
        g.transform.position = new Vector3(worldX,worldY,0);

    }

    public Sprite b2;
    public Sprite b4;
    public Sprite b8;
    public Sprite b16;
    public Sprite b32;
    public Sprite b64;
    public Sprite b128;
    public Sprite b256;
    public Sprite b512;
    public Sprite b1024;
    public Sprite b2048;
    public SpriteRenderer[] map;
    public void Display()
    {
        for(int i = 0; i < 4;i++)
        {
            for(int j = 0; j < 4; j++)
            {
                switch (WorldMap[i, j].tag)
                {
                    case 2:
                        break;
                    case 4:
                        break;
                    case 8:
                        break;
                    case 16:
                        break;
                    case 32:
                        break;
                    case 64:
                        break;
                    case 128:
                        break;
                    case 256:
                        break;
                    case 512:
                        break;
                    case 1024:
                        break;
                    case 2048:
                        break;

                }

            }
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


    public void WorldMove(int dir)
    {
        if (dir == 3)
        {
            for(int x = 1; x < 4; x++)//逐x
            {
                for(int y=1;y< 4;y++)//逐y,逐格
                {
                    //去获取它的相邻格
                    for (int i =  x-1; i >= 0; i--)//从自己向边缘，逐x
                    {
                        if (WorldMap[i, y].tag != 0)
                        {
                            WorldMap[x, y].tag = 0;
                            WorldMap[i + 1, y].tag = WorldMap[x, y].tag;
                            WorldMap[x, y].target = i + 1;


                            if (WorldMap[i, y].tag == WorldMap[x, y].tag)//相邻的格子和自己点数相同
                            {
                                WorldMap[x, y].tag = 0;//移除当前格
                                WorldMap[i, y].tag = WorldMap[x, y].tag * 2;//合并格子
                                WorldMap[x, y].target = i;
                            }

                            break;
                        }
                }
                }
            }
        }
        OnWorldMoveDoneEvent?.Invoke();
    }






    public bool IfMovable()
    {
        return false;
    }

    public void End()
    {
    }

    
}
