using System;
using UnityEngine;

public class BossTurnState : GameState
{
    public BossTurnState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() {
        Console.WriteLine("进入Boss回合阶段");
        currentCoroutine = StartCoroutine(DelayedInit());
        GameManager.Instance.StartTurn();
    }

    public override void Update() {

    }

    public override void Exit() {
        Console.WriteLine("退出Boss回合阶段");
    }

    private System.Collections.IEnumerator DelayedInit() {
        if (GameData.IsBossSpawned) {
            Debug.Log("进行Boss回合阶段的协程");
            BossManager.Instance.UpdateBossData();
            DeckManager.Instance.BossAttack();
        }

        yield return null;
    }
}

