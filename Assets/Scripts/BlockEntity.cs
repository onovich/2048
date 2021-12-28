using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBlockEntity
{
    void Init();//初始化方块，包含初始化动画
    void Remove();//移除方块
    void RefreshSprite(Sprite[] sprites);

    int blockTag { get; set; }
    Vector2Int? target { get; set; }

}


public class BlockEntity : MonoBehaviour,IBlockEntity
{
    public int blockTag { get; set; }
    public Vector2Int? target { get; set; }

    public void RefreshSprite(Sprite[] sprites)
    {
        float bt = blockTag;
        GetComponent<SpriteRenderer>().sprite = sprites[(int)Mathf.Log(bt, 2) - 1];
        //Debug.Log("已更新点数显示");
    }

    private void Start()
    {
        Init();
    }


    public void Init()
    {
        float start = .5f;
        transform.localScale = new Vector3(start,start,1);
        StartCoroutine(Fadein());
    }

    IEnumerator Fadein()
    {
        float start = .5f;
        float end = 1f;
        float current = start;
        while (current < end)
        {
            current = Mathf.Lerp(start,end, current/end);
            transform.localScale = new Vector3(current, current, 1);
            current += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1,1, 1);

    }


    public void Remove()
    {
        //Destroy(gameObject);
        DestroyImmediate(gameObject);
    }

    private void OnMouseDown()
    {
        Debug.Log("被点击到的是:"+gameObject.name+",点数为"+blockTag);
    }



}
