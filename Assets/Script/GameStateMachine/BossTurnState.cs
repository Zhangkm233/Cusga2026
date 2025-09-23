using System;
using UnityEngine;

public class BossTurnState : GameState
{
    public BossTurnState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() {
        Console.WriteLine("进入BOSS回合阶段");

    }

    public override void Update() {

    }

    public override void Exit() {
        Console.WriteLine("退出BOSS回合阶段");
    }
}

