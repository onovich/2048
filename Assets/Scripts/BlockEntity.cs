﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockEntity
{
    void Init();//初始化方块，包含初始化动画
    void Move();//移动方块
    void Remove();//移除方块

    int blockTag { get; set; }
    Vector2Int? target { get; set; }

}


public class BlockEntity : MonoBehaviour,IBlockEntity
{
    public int blockTag { get; set; }
    public Vector2Int? target { get; set; }



    public void Init()
    {
        throw new System.NotImplementedException();
    }

    public void Move()
    {
        throw new System.NotImplementedException();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
