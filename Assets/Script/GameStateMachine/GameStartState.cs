using System;
using UnityEngine;

public class GameStartState : GameState
{
    public GameStartState(GameStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter() {
        Console.WriteLine("进入游戏开始阶段");
        DeckManager.Instance.ShuffleDeck();
        var cameraController = CameraController.Instance;
        if (cameraController != null) {
            cameraController.transform.localRotation = Quaternion.Euler(11.5f,0f,0f);
        } else {
            Debug.LogWarning("CameraController.Instance is missing when entering GameStartState.");
        }
        GameManager.Instance.StartTurn();

    }
    public override void Update() {
    }
    public override void Exit() {
        Console.WriteLine("退出游戏开始阶段");
    }
}
