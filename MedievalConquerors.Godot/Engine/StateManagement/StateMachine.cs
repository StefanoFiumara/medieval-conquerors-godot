﻿namespace MedievalConquerors.Engine.StateManagement;
    
public class StateMachine
{
    public IState CurrentState { get; private set; } = new NullGameState();
    public IState PreviousState { get; private set; }

    public StateMachine() { }

    public StateMachine(IState initialState)
    {
        ChangeState(initialState);
    }
    
    public void ChangeState<TState>() where TState : IState, new()
    {
        var toState = new TState();
        ChangeState(toState);
    }

    public void ChangeState(IState newState)
    {
        var fromState = CurrentState;
        if (fromState == newState) return;
            
        fromState.Exit();
            
        CurrentState = newState;
        PreviousState = fromState;

        newState.Enter();
    }
}