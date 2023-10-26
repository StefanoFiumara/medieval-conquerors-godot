using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class BoardSystem : GameComponent, IAwake, IDestroy
{
    private IEventAggregator _events;
    private IGameBoard _gameBoard;
    
    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _gameBoard = Game.GetComponent<IGameBoard>();
        
        _events.Subscribe<MoveUnitAction>(GameEvent.Perform<MoveUnitAction>(), OnPerformMoveUnit);
    }

    private void OnPerformMoveUnit(MoveUnitAction action)
    {
        var oldTile = _gameBoard.GetTile(action.CardToMove.BoardPosition);
        var newTile = _gameBoard.GetTile(action.TargetTile);

        oldTile.Objects.Remove(action.CardToMove);
        newTile.Objects.Add(action.CardToMove);
        
        action.CardToMove.BoardPosition = action.TargetTile;
    }

    public void Destroy()
    {
        
    }
}