using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class PlayerSystem : GameComponent, IAwake
{
    private IEventAggregator _events;
    private IGameBoard _gameBoard;
    private Match _match;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _gameBoard = Game.GetComponent<IGameBoard>();
        _match = Game.GetComponent<Match>();
        
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        
        _events.Subscribe<ShuffleDeckAction>(GameEvent.Perform<ShuffleDeckAction>(), OnPerformShuffleDeck);
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

    

    private void OnPerformShuffleDeck(ShuffleDeckAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        player.Deck.Shuffle();
    }
}