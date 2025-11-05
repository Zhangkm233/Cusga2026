using System;
using UnityEditor;
using UnityEngine;

public class TurnStartState : GameState
{
    public TurnStartState(GameStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter() {
        Console.WriteLine("进入回合开始阶段");
        Debug.LogWarning($"开始第{GameData.turnCount}回合");
        if(GameData.turnCount == 0) {
            DeckManager.Instance.TestAddCard();
        }
        DeckManager.Instance.DrawPhase();
        //额外抽卡阶段-按照额外抽卡效果，额外抽取对应卡牌
        DeckManager.Instance.ExtraDrawPhase();
        UIManager.Instance.PlayDealAnimation();
    }
    public override void Update() {
        if(GameData.IsCardAnimationPlaying == false) {
            Debug.Log("卡牌发牌动画结束，进入玩家回合");
            GameManager.Instance.stateMachine.ChangePhase(GamePhase.PlayerTurn);
        }
    }
    public override void Exit() {
        Console.WriteLine("退出回合开始状态");
    }
}
