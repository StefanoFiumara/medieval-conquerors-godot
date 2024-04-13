using System;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class BoardSystem : GameComponent, IAwake
{
    private IEventAggregator _events;
    private IGameBoard _gameBoard;
    private Match _match;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _gameBoard = Game.GetComponent<IGameBoard>();
        _match = Game.GetComponent<Match>();
        
        _events.Subscribe<MoveUnitAction>(GameEvent.Perform<MoveUnitAction>(), OnPerformMoveUnit);
        _events.Subscribe<MoveUnitAction, ActionValidatorResult>(GameEvent.Validate<MoveUnitAction>(), OnValidateMoveUnit);
        _events.Subscribe<ChangeTurnAction>(GameEvent.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
    }

    private void OnValidateMoveUnit(MoveUnitAction action, ActionValidatorResult validator)
    {
        var moveAttr = action.CardToMove.GetAttribute<MovementAttribute>();
        
        if(action.CardToMove.Zone != Zone.Board)
            validator.Invalidate("Card is not on the board.");
        
        if(moveAttr == null)
            validator.Invalidate("Card does not have MoveAttribute.");

        else if (!moveAttr.CanMove(_gameBoard.Distance(action.CardToMove.BoardPosition, action.TargetTile)))
            validator.Invalidate("Card's MoveAttribute does not have enough distance remaining.");
    }

    private void OnPerformMoveUnit(MoveUnitAction action)
    {
        var oldTile = _gameBoard.GetTile(action.CardToMove.BoardPosition);
        var newTile = _gameBoard.GetTile(action.TargetTile);

        oldTile.Objects.Remove(action.CardToMove);
        newTile.Objects.Add(action.CardToMove);
        
        var distanceTraveled = _gameBoard.Distance(action.CardToMove.BoardPosition, action.TargetTile);
        
        action.CardToMove.BoardPosition = action.TargetTile;
        var moveAttr = action.CardToMove.GetAttribute<MovementAttribute>();
        moveAttr.Move(distanceTraveled);
    }
    
    private void OnPerformChangeTurn(ChangeTurnAction action)
    {
        var player = _match.Players[action.NextPlayerId];
        foreach (var card in player.Board)
        {
            foreach (var attr in card.Attributes)
            {
                attr.Reset();
            }
        }
    }
}