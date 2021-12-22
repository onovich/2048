using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockEntity
{
    void Init();//初始化方块，包含初始化动画
    void Move();//移动方块
    void Remove();//移除方块

}


public class BlockEntity : MonoBehaviour, IBlockEntity
{
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
        throw new System.NotImplementedException();
    }
}
