using System;
using UnityEngine;

public class GameStartState : GameState
{
    public GameStartState(GameStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter() {
        Console.WriteLine("进入游戏开始阶段");
        // 初始化游戏数据
        DeckManager.Instance.ShuffleDeck();
        //DeckManager.Instance.DrawCard();
        GameManager.Instance.StartTurn();
    }
    public override void Update() {
    }
    public override void Exit() {
        Console.WriteLine("退出游戏开始状态");
    }
}
