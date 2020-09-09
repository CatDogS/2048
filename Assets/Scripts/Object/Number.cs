using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    private Image bg;
    private Text number_text;

    private MyGrid inGrid;//这个数字所在的格子

    public NumberStatus status;

    private float spawnScaleTime = 1;
    private bool isPlayingSpawnAnim = false;

    private float mergeScaleTime = 1;
    private float mergeScaleTimeBack = 1;

    private bool isPlayingMergeAnim = false;

    private void Awake()
    {
        bg = transform.GetComponent<Image>();
        number_text = transform.Find("Text").GetComponent<Text>();
    }

    //初始化
    public void Init(MyGrid myGrid)
    {
        myGrid.SetNumber(this);
        //设置所在的格子
        this.SetGrid(myGrid);
        //给一个初始化数字
        this.SetNumber(2);
        status = NumberStatus.Normal;

        transform.localScale = Vector2.zero;
        PlaySpawnAnim();
    }

    //设置格子
    public void SetGrid(MyGrid myGrid) {
        this.inGrid = myGrid;
    }

    //获取格子
    public MyGrid GetGrid() { return this.inGrid; }

    //设置数字
    public void SetNumber(int number) {
        this.number_text.text = number.ToString();
    }

    //获取数组
    public int GetNumber() { return int.Parse(number_text.text); }

    //把这个数字移动到某一个格子的下面
    public void MoveToGrid(MyGrid myGrid)
    {
        transform.SetParent(myGrid.transform);
        transform.localPosition = Vector3.zero;

        this.GetGrid().SetNumber(null);

        //设置格子
        myGrid.SetNumber(this);
        this.SetGrid(myGrid);
    }

    //合并
    public void Merge()
    {
        GamePanel gamePanel = GameObject.Find("Canvas/GamePanel").GetComponent<GamePanel>();
        gamePanel.AddScore(this.GetNumber());
        //this.GetNumber()//要增加的分数

        int number = this.GetNumber() * 2;

        this.SetNumber(this.GetNumber() * 2);
        status = NumberStatus.NotMerge;
    } 

    //判断能不能合并
    public bool IsMerge(Number number)
    {
        if (this.GetNumber() == number.GetNumber() && number.status == NumberStatus.Normal)
        {
            return true;
        }
        return false;
    }

    public void PlaySpawnAnim()
    {
        spawnScaleTime = 0;
        isPlayingSpawnAnim = true;
    }

    public void PlayMergeAnim()
    {
        mergeScaleTime = 0;
        mergeScaleTimeBack = 0;
        isPlayingMergeAnim = true;
    }

    public void Update()
    {
        //创建动画
        if (isPlayingSpawnAnim)
        {
            if (spawnScaleTime <= 1)
            {
                spawnScaleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, spawnScaleTime);

            }
            else
            {
                isPlayingSpawnAnim = false;
            }
        }


        //合并动画
        if (isPlayingMergeAnim) {
            if (mergeScaleTime <= 1 && mergeScaleTimeBack == 1) //变大的过程
            {
                mergeScaleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, mergeScaleTime);

            }
            if (mergeScaleTime >= 1 && mergeScaleTimeBack <= 1)
            {
                mergeScaleTime += Time.deltaTime * 4;
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, mergeScaleTimeBack);

            }
        }
        if (mergeScaleTime >= 1 && mergeScaleTimeBack >= 1)
        {
            isPlayingMergeAnim = false;
        }
    }

    
}
