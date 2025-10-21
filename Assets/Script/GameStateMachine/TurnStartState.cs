using System;
using UnityEngine;

public class TurnStartState : GameState
{
    public TurnStartState(GameStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter() {
        Console.WriteLine("进入回合开始阶段");

        DeckManager.Instance.TestAddCard();
        DeckManager.Instance.DrawPhase();
        //额外抽卡阶段-按照额外抽卡效果，额外抽取对应卡牌
        DeckManager.Instance.ExtraDrawPhase();
        UIManager.Instance.PlayDealAnimation();
        GameManager.Instance.stateMachine.ChangePhase(GamePhase.PlayerTurn);
    }
    public override void Update() {
    }
    public override void Exit() {
        Console.WriteLine("退出回合开始状态");
    }
}
