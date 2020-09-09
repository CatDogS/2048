using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    //[SerializeField] GameObject SelectModelPanel;
    public SelectModelPanel selectModelPanel;
    public SetPanel setPanel;
    //点击开始游戏
    public void OnStartGameClick() {
        //显示选择模式的界面
        selectModelPanel.Show();
    }

    //点击设置
    public void OnSetClick() {
        //显示设置的界面
        setPanel.Show();
    }

    //点击退出游戏
    public void OnExitClick() {
        //退出游戏
        Application.Quit();
    }
}
