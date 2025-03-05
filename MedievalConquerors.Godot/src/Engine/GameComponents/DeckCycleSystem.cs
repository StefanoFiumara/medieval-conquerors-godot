using System;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class DeckCycleSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private Match _match;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();

        _events.Subscribe<DrawCardsAction>(GameEvent.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
        _events.Subscribe<CycleDeckAction>(GameEvent.Perform<CycleDeckAction>(), OnPerformCycleDeck);
    }

    private void OnPerformCycleDeck(CycleDeckAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        player.MoveCards(player.Discard.ToList(), Zone.Deck);
    }

    private void OnPrepareDrawCards(DrawCardsAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        if (player.Deck.Count < action.Amount)
        {
            // If the deck doesn't have enough cards for the draw action
            // cancel the action draw the remainder of the deck
            // then, cycle the deck (shuffle the discard pine into the deck), and draw the remaining cards.
            action.Cancel();
            Game.AddReaction(new DrawCardsAction(action.TargetPlayerId, player.Deck.Count));
            Game.AddReaction(new CycleDeckAction(player.Id));
            Game.AddReaction(new ShuffleDeckAction(action.TargetPlayerId));
            var remaining = Math.Min(action.Amount - player.Deck.Count, player.Discard.Count);
            if(remaining > 0)
                Game.AddReaction(new DrawCardsAction(action.TargetPlayerId, remaining));
        }
    }
}
