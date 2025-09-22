using System;
using UnityEngine;

// 玩家回合
public class PlayerTurnState : GameState
{
    public PlayerTurnState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() {
        Console.WriteLine("进入玩家回合阶段");
    }

    public override void Update() {

    }

    public override void Exit() {
        Console.WriteLine("退出玩家回合阶段");
    }
}