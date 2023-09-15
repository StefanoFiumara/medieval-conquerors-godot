using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.StateManagement;
    
public class StateMachine
{
    public IState CurrentState { get; private set; } = new NullGameState();
    public IState PreviousState { get; private set; }

    public void ChangeState<TGameState>() where TGameState : IState, new()
    {
        var toState = new TGameState();
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