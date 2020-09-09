using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//[SerializeField]
public class SelectModelPanel : View
{
    public void OnSelectModelClick( int count ) {
        //选择模式
        PlayerPrefs.SetInt(Const.GameModel, count);
        //调整长度  ，到游戏场景
        //AsyncOperation asyncOperation = //进度条
        SceneManager.LoadSceneAsync(1);
        //asyncOperation.progress
    }

    //显示
    public void Show() {
        gameObject.SetActive(true);
    }

    //隐藏
    public void Hide() {
        gameObject.SetActive(false);
    }
}
