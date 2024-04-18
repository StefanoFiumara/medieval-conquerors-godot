using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input.InputStates;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Engine.StateManagement;

namespace MedievalConquerors.Engine.Input;

public class InputSystem : GameComponent, IAwake, IDestroy
{
    public const string ClickedEvent = "InputSystem.ClickedEvent";
    
    private StateMachine _stateMachine;
    private IEventAggregator _events;
    private ActionSystem _actionSystem;
    
    private ILogger _logger;
    private Match _match;
    
    public void Awake()
    {
        _actionSystem = Game.GetComponent<ActionSystem>();
        _match = Game.GetComponent<Match>();
        _events = Game.GetComponent<EventAggregator>();
        
        _events.Subscribe<IClickable>(ClickedEvent, OnInput);

        _logger = Game.GetComponent<ILogger>();
        
        _stateMachine = new StateMachine(new CardSelectionState(Game, _logger));
    }

    private void OnInput(IClickable selected)
    {
        if (_actionSystem.IsActive)
            return;

        // TODO: Should this check be at the state level?
        //       Some input states can be transitioned without it 
        //       needing to be the player's turn (e.g. preview discard pile)
        if (_match.CurrentPlayer != _match.LocalPlayer)
            return;
        
        if (_stateMachine.CurrentState is ITurnState turnState)
        {
            var newState = turnState.OnReceivedInput(selected);
            _stateMachine.ChangeState(newState);
        }   
    }

    public void Destroy()
    {
        _events.Unsubscribe(ClickedEvent, OnInput);
    }
}