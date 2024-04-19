using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class SequenceState : IState
{
    public void Enter() { }
    public void Exit() { }
}

public class IdleState : IState
{
    private readonly IGame _game;

    public IdleState(IGame game)
    {
        _game = game;
    }
    
    public void Enter()
    {
        _game.GetComponent<CardSystem>().Refresh();
        
        // TODO: Check AI System here to perform enemy action
        
    }
    public void Exit() { }
}

// TODO: Unit tests for this component
public class GlobalGameStateSystem : GameComponent, IAwake
{
    private readonly StateMachine _stateMachine = new();
    private EventAggregator _events;

    public IState CurrentState => _stateMachine.CurrentState;
    
    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        
        _events.Subscribe(ActionSystem.BeginSequenceEvent, OnBeginSequence);
        _events.Subscribe(ActionSystem.CompleteEvent, OnActionsComplete);
    }

    private void OnActionsComplete()
    {
        // TODO: Check for game over and switch to GameOverState here
        _stateMachine.ChangeState(new IdleState(Game));
    }

    private void OnBeginSequence()
    {
        _stateMachine.ChangeState<SequenceState>();
    }
}

// TODO: Unit tests for this component
public class CardSystem : GameComponent, IAwake
{
    private readonly List<Card> _playable = new();

    private Match _match;
    private IMap _map;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<IMap>();
    }

    public bool IsPlayable(Card card) => _playable.Contains(card);

    public void Refresh()
    {
        _playable.Clear();
        
        foreach (var card in _match.CurrentPlayer.Hand)
        {
            // TODO: more robust check for available tiles for this card, query attributes?
            var randomTargetTile = _map.SearchTiles(t => t.Terrain == TileTerrain.Grass).ToList().GetRandom();
            var playAction = new PlayCardAction(card, randomTargetTile.Position);
            if (playAction.Validate(Game).IsValid)
            {
                _playable.Add(card);
            }
        }
    }
}