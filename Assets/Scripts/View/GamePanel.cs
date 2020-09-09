using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public Text text_score;//分数
    public Text text_best_score;//最高分
    public Button btn_last;  //上一步
    public Button btn_restart; //重新开始
    public Button btn_exit;  //退出
    
    public Transform gridParent;  //格子的父物体

    



    private int row;
    private int col;

    public MyGrid[][] grids = null;//所有的格子

    public GameObject gridPrefab;
    public GameObject numberPrefab;

    private Vector3 pointerDownPos, pointerUpPos;


    public List<MyGrid> canCreatNumberGrid = new List<MyGrid>(); //可以创建数字的格子

    private bool isNeedCreateNumber = false;

    public WinPanel winPanel;

    public int currentScore;

    public Dictionary<int, int> grid_config = new Dictionary<int, int>() { {4,430}, {5,350}, {6,290 } };
    public Dictionary<int, int> grid_spac = new Dictionary<int, int>() { { 4, -190 }, { 5, -160 }, { 6, -130} };
    //public Dictionary<int, int> grid_pad = new Dictionary<int, int>() { { 4, -47 }, { 5, -36 }, { 6, -30 } };


    //上一步
    public void OnLastClick() { }

    //重新开始
    public void OnRestartClick() {
        RestartGame();
    }

    //退出
    public void OnExitClick() {
        ExitGame();
    }

    private void ExitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }

    //初始化格子
    public void InitGrid() {

        //获取格子数目
        int gridNum = PlayerPrefs.GetInt(Const.GameModel, 4);
        GridLayoutGroup gridLayoutGroup = gridParent.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraintCount = gridNum;
        gridLayoutGroup.cellSize = new Vector2(grid_config[gridNum], grid_config[gridNum]);
        gridLayoutGroup.spacing = new Vector2(grid_spac[gridNum], grid_spac[gridNum]);
        if (gridNum == 4) {
            gridLayoutGroup.padding.left = -210;// new RectOffset(-47, 0, -47, 0);
            gridLayoutGroup.padding.top = -210;
        } else if (gridNum == 5)
        {
            gridLayoutGroup.padding.left = -175;// new RectOffset(-47, 0, -47, 0);
            gridLayoutGroup.padding.top = -175;
        } else if (gridNum == 6) {
            gridLayoutGroup.padding.left = -145;// new RectOffset(-47, 0, -47, 0);
            gridLayoutGroup.padding.top = -145;
        }

        //初始化格子
        grids = new MyGrid[gridNum][];

        //创建格子
        row = gridNum;
        col = gridNum;
        for (int i = 0; i <row; i++) {
            for (int j = 0; j < col; j++){
                if (grids[i] == null) {
                    grids[i] = new MyGrid[gridNum];
                }
                grids[i][j] = CreatGrid();
            }
        }
    }

    //创建格子
    public MyGrid CreatGrid() {
        //实例化格子预制体
        GameObject gameObject = GameObject.Instantiate(gridPrefab, gridParent);
        return gameObject.GetComponent<MyGrid>();
    }

    //创建数字
    public void CreatNumber() {
        //找到数字所在格子
        canCreatNumberGrid.Clear();
        for (int i = 0; i<row; i++) {
            for (int j = 0; j < col; j++)
            {
                if (!grids[i][j].IsHaveNumber())
                {//判断这个格子有没有数字
                    //如果没有数字
                    canCreatNumberGrid.Add(grids[i][j]);
                }
            }
        }
        if (canCreatNumberGrid.Count == 0) { return; }

        //随机一个格子
        int index = Random.Range(0, canCreatNumberGrid.Count);

        //创建数字并放入格子
        GameObject gameObj = GameObject.Instantiate(numberPrefab, canCreatNumberGrid[index].transform);
        gameObj.GetComponent<Number>().Init(canCreatNumberGrid[index]);  //进行初始化
    }

    private void Awake()
    {
        //初始化界面信息
        InitPanelMessage();
        //初始化格子
        InitGrid();

        //创建第一个数字
        CreatNumber();
    }

    public void OnPointerDown() {
        //Debug.Log("按下：" + Input.mousePosition);
        pointerDownPos = Input.mousePosition;
    }

    public void OnPointerUp()
    {
        //Debug.Log("抬起：" + Input.mousePosition);
        pointerUpPos = Input.mousePosition;

        if (Vector3.Distance(pointerDownPos, pointerUpPos) < 50) {
            Debug.Log("无效的操作");
            return;
        }

        MoveType moveType = CaculateMoveType();
        Debug.Log("移动类型" + moveType);
        MoveNumber(moveType);

        //产生数字
        if (isNeedCreateNumber)
        {
            CreatNumber();
        }

        //把所有数字的状态 恢复成正常状态
        ResetNumberStatus();
        isNeedCreateNumber = false;

        //判断游戏是否结束
        if (IsGameOver())
        {
            GameOver();
        }
    }

    public MoveType CaculateMoveType()
    {
        if (Mathf.Abs(pointerUpPos.x - pointerDownPos.x) > Mathf.Abs(pointerUpPos.y - pointerDownPos.y))
        {
            //左右移动
            if (pointerUpPos.x - pointerDownPos.x > 0)
            {
                //向右移动
                //Debug.Log("向右移动");
                return MoveType.RIGHT;
            }
            else
            {
                //向左移动
                //Debug.Log("向左移动");
                return MoveType.LEFT;
            }
        }
        else
        {
            //上下移动
            if (pointerUpPos.y - pointerDownPos.y > 0)
            {
                //向下移动
                //Debug.Log("向上移动");
                return MoveType.TOP;
            }
            else
            {
                //向左移动
                //Debug.Log("向下移动");
                return MoveType.DOWN;
            }
        }
    }

    public void MoveNumber(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.TOP:
                for (int j = 0; j < col; j++)
                {
                    for (int i = 1; i < row; i++)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {
                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("移动坐标"+i+","+j);
                            for (int m = i - 1; m >= 0; m--)
                            {
                                //if (grids[m][j].IsHaveNumber())
                                //{
                                //    //判断能不能合并
                                //    if (number.GetNumber() == grids[m][j].GetNumber().GetNumber())
                                //    {
                                //        //合并
                                //        grids[m][j].GetNumber().Merge();
                                //        //销毁当前的这个数字
                                //        number.GetGrid().SetNumber(null);
                                //        GameObject.Destroy(number.gameObject);
                                //    }
                                //    break;
                                //}
                                //else
                                //{
                                //    //没数字  移动上去
                                //    number.MoveToGrid(grids[m][j]);
                                //    number.GetComponent<RectTransform>().offsetMax = new Vector2(0,0);
                                //    number.GetComponent<RectTransform>().offsetMin = new Vector2(0,0);
                                //}
                                Number targetNumber = null;
                                if (grids[m][j].IsHaveNumber())
                                {
                                    targetNumber = grids[m][j].GetNumber();
                                }

                                HandleNumber(number, targetNumber, grids[m][j]);

                                if(targetNumber != null)
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
                break;
            case MoveType.DOWN:
                for (int j = 0; j < col; j++)
                {
                    for (int i = row - 2; i >= 0; i--)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("移动坐标"+i+","+j);
                            for (int m = i + 1; m < row; m++)
                            {
                                Number targetNumber = null;
                                if (grids[m][j].IsHaveNumber())
                                {
                                    targetNumber = grids[m][j].GetNumber();
                                }

                                HandleNumber(number, targetNumber, grids[m][j]);

                                if (targetNumber != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case MoveType.LEFT:
                for (int i = 0; i < row; i++)
                {
                    for (int j = 1; j < col; j++)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("移动坐标"+i+","+j);
                            for (int m = j - 1; m >= 0; m--)
                            {
                                Number targetNumber = null;
                                if (grids[i][m].IsHaveNumber())
                                {
                                    targetNumber = grids[i][m].GetNumber();
                                }

                                HandleNumber(number, targetNumber, grids[i][m]);

                                if (targetNumber != null)
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
                break;
            case MoveType.RIGHT:
                for (int i = 0; i< row; i++)
                {
                    for (int j = col - 2; j >= 0; j--)
                    {
                        if (grids[i][j].IsHaveNumber())
                        {

                            Number number = grids[i][j].GetNumber();

                            //Debug.Log("移动坐标"+i+","+j);
                            for (int m = j + 1; m < col; m++)
                            {
                                Number targetNumber = null;
                                if (grids[i][m].IsHaveNumber())
                                {
                                    targetNumber = grids[i][m].GetNumber();
                                }

                                HandleNumber(number, targetNumber, grids[i][m]);

                                if (targetNumber != null)
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    public void HandleNumber(Number current, Number target, MyGrid targetGrid) {
        if (target != null)
        {
            //判断能不能合并
            if (current.IsMerge(target)) {
                target.Merge();


                //销毁当前的数字
                current.GetGrid().SetNumber(null);
                GameObject.Destroy(current.gameObject);
                isNeedCreateNumber = true;
            }
        }
        else
        {
            //没有数字
            current.MoveToGrid(targetGrid);
            current.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            current.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            isNeedCreateNumber = true;
        }
    }

    public void ResetNumberStatus()
    {
        //遍历所有的数字
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grids[i][j].IsHaveNumber())
                {//判断这个格子有没有数字
                    //如果有数字
                    grids[i][j].GetNumber().status = NumberStatus.Normal;
                }
            }
        }
    }

    //判断游戏是否结束
    public bool IsGameOver()
    {
        //判断格子是否满了
        for (int i = 0; i<row; i++)
        {
            for (int j = 0; j<col; j++)
            {
                if (!grids[i][j].IsHaveNumber())
                {
                    return false;
                }
            }
        }
        //判断有没有数字合并
        for (int i =0; i<row;i++)
        {
            for (int j=0; j<col; j++)
            {
                MyGrid up = IsHaveGrid(i - 1, j)? grids[i - 1][j]:null;
                MyGrid down = IsHaveGrid(i + 1, j) ? grids[i + 1][j] : null;
                MyGrid left = IsHaveGrid(i, j - 1) ? grids[i][j - 1] : null;
                MyGrid right = IsHaveGrid(i, j + 1) ? grids[i][j + 1] : null;

                if (up!=null)
                {
                    if (grids[i][j].GetNumber().IsMerge(up.GetNumber()))
                    {
                        return false;
                    }
                }
                if (down != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(down.GetNumber()))
                    {
                        return false;
                    }
                }
                if (left != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(left.GetNumber()))
                    {
                        return false;
                    }
                }
                if (right != null)
                {
                    if (grids[i][j].GetNumber().IsMerge(right.GetNumber()))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public bool IsHaveGrid(int i, int j)
    {
        if (i >= 0 && i <row && j>=0 && j<col) {
            return true;
        }
        return false;
    }

    public void GameOver()
    {
        Debug.Log("游戏结束");
        winPanel.Show();
    }

    public void RestartGame()
    {
        //清空数据
        //清空分数
        ResetScore();
        //清空数字
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (grids[i][j].IsHaveNumber())
                {
                    GameObject.Destroy(grids[i][j].GetNumber().gameObject);
                    grids[i][j].SetNumber(null);
                }
                
            }
        }
        //创建一个数字
        CreatNumber();
    }


    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScore(currentScore);

        //判断当前分数是不是最高分数
        if (currentScore>PlayerPrefs.GetInt(Const.BestScore, 0))
        {
            PlayerPrefs.SetInt(Const.BestScore, currentScore);
            UpdateBestScore(currentScore);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScore(currentScore);
    }

    public void UpdateScore(int score)
    {
        this.text_score.text = score.ToString();
    }

    public void UpdateBestScore(int bestscore)
    {
        this.text_best_score.text = bestscore.ToString();
    }

    public void InitPanelMessage()
    {
        this.text_best_score.text = PlayerPrefs.GetInt(Const.BestScore, 0).ToString();
    }
}
