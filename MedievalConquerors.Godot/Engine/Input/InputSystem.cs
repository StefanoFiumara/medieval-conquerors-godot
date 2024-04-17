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

    public void Awake()
    {
        _actionSystem = Game.GetComponent<ActionSystem>();
        _events = Game.GetComponent<EventAggregator>();
        _events.Subscribe<IClickable>(ClickedEvent, OnInput);

        _logger = Game.GetComponent<ILogger>();
        
        // TODO: Define initial state
        _stateMachine = new StateMachine(new CardSelectionState(Game, _logger));
    }

    private void OnInput(IClickable selected)
    {
        if (_actionSystem.IsActive)
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

public class CardSelectionState : ITurnState
{
    private readonly IGame _game;
    private readonly ILogger _logger;

    private readonly CardSystem _cardSystem;

    public CardSelectionState(IGame game, ILogger logger)
    {
        _game = game;
        _logger = logger;

        _cardSystem = _game.GetComponent<CardSystem>();
    }
    
    public void Enter() { }
    public void Exit() { }

    public ITurnState OnReceivedInput(IClickable clickedObject)
    {
        if (clickedObject is not Card c)
            return this;

        _logger.Info($"clicked on card: {c.CardData.Title}");
        
        if (_cardSystem.IsPlayable(c))
        {
            // TODO: switch to TileSelectionState
            // pass selected card as parameter to compose final PlayCardAction later
        }
        
        return this;
    }
}