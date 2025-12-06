using System;
using UnityEngine;

public class GamePausedState : GameState
{
    public GamePausedState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() {
        Console.WriteLine("进入游戏停止阶段");
    }

    public override void Update() {
    }

    public override void Exit() {
        Console.WriteLine("退出游戏停止阶段");
    }
}