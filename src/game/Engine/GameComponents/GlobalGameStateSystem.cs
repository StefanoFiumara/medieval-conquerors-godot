using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data.GameStates;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this component
public class GlobalGameStateSystem : GameComponent, IAwake
{
    private readonly StateMachine _stateMachine = new();
    private EventAggregator _events;

    public IState CurrentState => _stateMachine.CurrentState;
    
    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        
        _events.Subscribe(ActionSystem.BEGIN_ACTION_EVENT, OnBeginSequence);
        _events.Subscribe(ActionSystem.COMPLETE_ACTION_EVENT, OnActionsComplete);
    }

    private void OnActionsComplete()
    {
        // TODO: Check for game over and switch to GameOverState here
        _stateMachine.ChangeState(new IdleState(Game));
    }

    private void OnBeginSequence()
    {
        _stateMachine.ChangeState<BusyState>();
    }
}

public static class GlobalGameStateExtensions
{
    public static bool IsIdle(this IGame game) => game.GetComponent<GlobalGameStateSystem>().CurrentState is IdleState;
}