using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAPP : MonoBehaviour
{
   

    public PlayerController PlayerController;
    public World World;
    public Game Game;
    public ScoreManager ScoreManager;


    public GameUI GameUI;

    //输入（按键）->世界（全局移动）->世界（生成）
    //输入->Game（退出游戏）
    //UI（点击）->输入（点击）->游戏（重启游戏）
    //世界->BlockEntity(单位移动、单位合成)
    //世界->Game（游戏结束）
    //世界->Score（计分）
    //Score->UI（刷新分数）
    private void Awake()
    {
        Game.StartGame();
        //世界订阅输入
        PlayerController.OnMoveEvent += World.WorldAction;
        PlayerController.OnEscEvent += Game.GameOver;
        World.OnWorldMoveDoneEvent += World.Generate;
        //Game订阅输入
        PlayerController.OnEscEvent += Game.QuitGame;
        GameUI.OnClickRestartEvent += PlayerController.Click;
        PlayerController.OnRestartEvent += Game.Restart;


        //Block移动由世界控制
        //Game订阅世界
        World.OnEndEvent += Game.GameOver;
        //Score订阅世界
        World.OnWorldMoveDoneEvent += ScoreManager.RefreshScore;
        World.OnWorldMoveDoneEvent += ScoreManager.RefreshBest;
        World.OnWorldMoveDoneEvent += ScoreManager.RefreshSteps;
        //UI订阅Score
        ScoreManager.OnScoreRefreshEvent += GameUI.RefreshScore;

    }

    private void Update()
    {
        PlayerController.Press();

    }


}
