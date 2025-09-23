using System;
using UnityEngine;

public class TurnEndState : GameState
{
    public TurnEndState(GameStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter() {
        Console.WriteLine("进入回合结束阶段");
        // 天灾阶段-回合结束时，销毁手中剩余的资源卡和技能卡，如果剩余了天灾卡则触发天灾
        DeckManager.Instance.ClearHand();
        // 动物阶段 - 所有活着的动物移动到自身所在格与相邻四格内所有空旷的格子中，最接近自己偏好地势数值的地块
        MapManager.Instance.AnimalPhase();
        // 充能阶段-仅当boss战未触发时执行，使场上所有地块获得1充能，如果有充能已满的地块，触发其效果
        MapManager.Instance.EnergyPhase();
        // 额外收益阶段 - 如果有每回合触发的额外收益，在此时进行判断并执行
        MapManager.Instance.ExtraEffectPhase();
        //Boss攻击阶段 - 仅当boss战触发时执行，boss进行攻击
        //回合结束阶段 - 执行回合结束扳机，最后回合数 + 1，如果回合已满则出现boss 还有随机添加天灾的效果
        //随机添加天灾 从第三回合开始，每回合有35%概率获得一张天灾（进入资源牌库）如果没有获得，则下次获得概率+20%
        if (GameData.turnCount >= 3) {
            if (GameData.IsRandomEventTriggered(GameData.disasterPercent)) {
                Debug.Log("获得一张天灾卡");
                DeckManager.Instance.AddCardToDeck(new DisasterCard());
            } else {
                GameData.disasterPercent += 20;
                Debug.Log($"概率上升到{GameData.disasterPercent}");
            }
        }
        GameData.turnCount++;
        if(GameData.turnCount > GameData.bossSpawnTurn) {
            GameManager.Instance.stateMachine.ChangePhase(GamePhase.BossTurn);
            return;
        }
        GameManager.Instance.StartTurn();
    }
    public override void Update() {
    }
    public override void Exit() {
        Console.WriteLine("退出回合结束状态");
    }
}
