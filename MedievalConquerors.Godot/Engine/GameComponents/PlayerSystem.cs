using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class PlayerSystem : GameComponent, IAwake, IDestroy
{
    private IEventAggregator _events;
    private IGameBoard _gameBoard;
    
    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _gameBoard = Game.GetComponent<IGameBoard>();
        
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        action.SourcePlayer.MoveCard(action.CardToPlay, Zone.Board);
        action.CardToPlay.BoardPosition = action.TargetTile;
        
        var tile = _gameBoard.GetTile(action.TargetTile);
        tile.Objects.Add(action.CardToPlay);
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        foreach (var card in action.CardsToDiscard) 
        {
            if (card.Zone == Zone.Board)
            {
                var tile = _gameBoard.GetTile(card.BoardPosition);
                tile.Objects.Remove(card);
            }
        }
        
        action.Target.MoveCards(action.CardsToDiscard, Zone.Discard);
    }
    
    public void Destroy()
    {
        _events.Unsubscribe(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Unsubscribe(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
    }
}