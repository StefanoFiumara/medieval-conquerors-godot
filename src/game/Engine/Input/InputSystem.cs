using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input.InputStates;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Input;

public class InputSystem : GameComponent, IAwake, IDestroy
{
    public const string CLICKED_EVENT = "InputSystem.ClickedEvent";

    private StateMachine _stateMachine;
    private EventAggregator _events;
    private ActionSystem _actionSystem;

    public void Awake()
    {
        _actionSystem = Game.GetComponent<ActionSystem>();
        _events = Game.GetComponent<EventAggregator>();

        _events.Subscribe<IClickable, InputEventMouseButton>(CLICKED_EVENT, OnInput);

        _stateMachine = new StateMachine(new IdleInputState(Game));
    }

    private void OnInput(IClickable selected, InputEventMouseButton mouseEvent)
    {
        if (_actionSystem.IsActive)
            return;

        if (_stateMachine.CurrentState is IClickableState turnState)
        {
            var newState = turnState.OnReceivedInput(selected, mouseEvent);
            _stateMachine.ChangeState(newState);
        }
    }

    public void Destroy()
    {
        _events.Unsubscribe(CLICKED_EVENT, OnInput);
    }
}
