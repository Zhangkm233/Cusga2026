using Unity.VisualScripting;
using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public interface IStateMachine
{
    void ChangeState(IState newState);
    void Update();
    IState CurrentState { get; }
}

public class StateMachine : IStateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState) {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Update() {
        CurrentState?.Update();
    }
}