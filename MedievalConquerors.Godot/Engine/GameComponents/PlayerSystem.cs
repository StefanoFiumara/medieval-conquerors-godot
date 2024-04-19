using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class PlayerSystem : GameComponent, IAwake
{
    private IEventAggregator _events;
    private Match _match;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();
        
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<ShuffleDeckAction>(GameEvent.Perform<ShuffleDeckAction>(), OnPerformShuffleDeck);
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        action.Target.MoveCards(action.CardsToDiscard, Zone.Discard);
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        action.SourcePlayer.MoveCard(action.CardToPlay, Zone.Map);
    }

    private void OnPerformShuffleDeck(ShuffleDeckAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        player.Deck.Shuffle();
    }
}