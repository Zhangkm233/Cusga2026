using System;
using Unity.VisualScripting;
using UnityEngine;

public class TurnEndState : GameState
{
    public TurnEndState(GameStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter() {
        Console.WriteLine("进入回合结束阶段");
        currentCoroutine = StartCoroutine(DelayedInit());
        GameData.turnCount++;
        if(GameData.turnCount > GameData.bossSpawnTurn) {
            if(GameData.IsBossSpawned == false) {
                BossManager.Instance.SpawnBoss();
                GameData.IsBossSpawned = true;
            }
            Debug.LogWarning($"结束第{GameData.turnCount - 1}回合，boss已生成");
            GameManager.Instance.stateMachine.ChangePhase(GamePhase.BossTurn);
        } else {
            Debug.LogWarning($"结束第{GameData.turnCount - 1}回合，boss未生成");
            GameManager.Instance.StartTurn();
        }
    }
    public override void Update() {
    }
    public override void Exit() {
        Console.WriteLine("退出回合结束状态");
    }

    private System.Collections.IEnumerator DelayedInit() {
        // 天灾阶段-回合结束时，销毁手中剩余的资源卡
        // 把技能卡洗回牌库，如果剩余了天灾卡则触发天灾
        DeckManager.Instance.DisasterPhase();
        // 动物阶段 - 所有活着的动物移动到自身所在格与相邻四格内所有空旷的格子中，最接近自己偏好地势数值的地块
        MapManager.Instance.AnimalPhase();
        if(!GameData.IsBossSpawned) {
            // 充能阶段-仅当boss战未触发时执行，使场上所有地块获得1充能，如果有充能已满的地块，触发其效果
            MapManager.Instance.EnergyPhase();
            // 额外收益阶段-仅当boss战未触发时执行，如果有每回合触发的额外收益，在此时进行判断并执行
            MapManager.Instance.ExtraEffectPhase();
        } else {
            Debug.Log("Boss战已触发，跳过充能和额外收益阶段");
        }
        //Boss攻击阶段（在bossTurnState里面实现） - 仅当boss战触发时执行，boss进行攻击
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
        Debug.Log("协程进行完毕");
        yield return null;
    }
}
