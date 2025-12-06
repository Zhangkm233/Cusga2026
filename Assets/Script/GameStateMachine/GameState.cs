using UnityEngine;

using System;
using System.Collections.Generic;
using System.Collections;

// 游戏阶段枚举
public enum GamePhase
{
    GameStart,   // 游戏开始 初始化数据
    TurnStart,   // 回合开始 执行一些回合开始的逻辑
    PlayerTurn,   // 玩家回合 玩家在这个阶段才可以进行操作
    TurnEnd,     // 回合结束 执行一些回合结束的逻辑
    GamePaused,   // 游戏暂停 玩家无法对游戏内进行操作
    BossTurn,   // Boss回合 Boss进行攻击（考虑到动画演出，所以预留一个）
}

// 基础游戏状态
public abstract class GameState : IState
{
    protected GameStateMachine stateMachine;

    protected Coroutine currentCoroutine;

    public GameState(GameStateMachine stateMachine) {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();



    // 启动协程的方法
    protected Coroutine StartCoroutine(IEnumerator coroutine) {
        return CoroutineHandler.Instance.StartCoroutine(coroutine);
    }

    // 停止协程的方法
    protected void StopCoroutine(Coroutine coroutine) {
        if (coroutine != null) {
            CoroutineHandler.Instance.StopCoroutine(coroutine);
        }
    }
}

// 游戏状态机
public class GameStateMachine
{
    private StateMachine stateMachine;
    private Dictionary<GamePhase,GameState> states;

    public GameStateMachine() {
        stateMachine = new StateMachine();
        states = new Dictionary<GamePhase,GameState>
        {
            { GamePhase.GameStart, new GameStartState(this) }, // 初始状态为玩家回合
            { GamePhase.PlayerTurn, new PlayerTurnState(this) },
            { GamePhase.TurnStart, new TurnStartState(this) },
            { GamePhase.TurnEnd, new TurnEndState(this) },
            { GamePhase.GamePaused, new GamePausedState(this) },
            { GamePhase.BossTurn, new BossTurnState(this)   }
        };
    }

    public void ChangePhase(GamePhase phase) {
        if (states.TryGetValue(phase,out GameState state)) {
            stateMachine.ChangeState(state);
        }
    }

    public void Update() {
        stateMachine.Update();
    }

    public GamePhase CurrentPhase {
        get {
            foreach (var pair in states) {
                if (pair.Value == stateMachine.CurrentState) {
                    return pair.Key;
                }
            }
            return GamePhase.GamePaused; // 默认值
        }
    }
}