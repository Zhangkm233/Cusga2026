using System;
using UnityEngine;

public class GamePausedState : GameState
{
    public GamePausedState(GameStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() {
        Console.WriteLine("½øÈëÓÎÏ·Í£Ö¹½×¶Î");
    }

    public override void Update() {
    }

    public override void Exit() {
        Console.WriteLine("ÍË³öÓÎÏ·Í£Ö¹½×¶Î");
    }
}
