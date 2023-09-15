using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input.InputStates;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Input;

public class InputSystem : GameComponent, IAwake, IDestroy
{
    public const string ClickedEvent = "InputSystem.ClickedEvent";
    public StateMachine StateMachine { get; private set; }
    private IEventAggregator _events;
    private ActionSystem _actionSystem;

    public void Awake()
    {
        StateMachine = new StateMachine();

        _actionSystem = Game.GetComponent<ActionSystem>();
        
        _events = Game.GetComponent<EventAggregator>();
        _events.Subscribe<IGameObject>(ClickedEvent, OnInput);
        
        // TODO: Define initial state
        // StateMachine.ChangeState(new CardSelectionState(_view));
    }

    public void Destroy()
    {
        _events.Unsubscribe(ClickedEvent, OnInput);
    }
    
    // TODO: Figure out signature for this handler
    private void OnInput(IGameObject selected)
    {
        if (_actionSystem.IsActive)
        {
            return;
        }

        if (StateMachine.CurrentState is ITurnState turnState)
        {
            var newState = turnState.OnReceivedInput(selected);
            StateMachine.ChangeState(newState);
        }   
    }
}