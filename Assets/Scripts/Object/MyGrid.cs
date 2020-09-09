using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    //当前格子的数字
    public Number number;

    //判断有没有数字
    public bool IsHaveNumber() {
        return number != null;
    }

    //获取格子的数字
    public Number GetNumber() { return number; }
    //设置数字
    public void SetNumber(Number number) { this.number = number; }
}
