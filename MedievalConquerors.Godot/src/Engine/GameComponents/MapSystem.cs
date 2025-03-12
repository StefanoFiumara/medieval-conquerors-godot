using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class MapSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private HexMap _map;
    private Match _match;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();
        _match = Game.GetComponent<Match>();

        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        var tile = _map.GetTile(action.TargetTile);

        if (action.CardToPlay.CardData.CardType == CardType.Building)
            tile.Building = action.CardToPlay;

        else if (action.CardToPlay.CardData.CardType == CardType.Unit)
        {
            if (tile.Building != null)
                Game.AddReaction(new GarrisonAction(action.CardToPlay, tile.Building));
            else
                tile.Unit = action.CardToPlay;
        }

        action.CardToPlay.MapPosition = action.TargetTile;
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        foreach (var card in action.CardsToDiscard)
        {
            if (card.MapPosition != HexMap.None)
            {
                var tile = _map.GetTile(card.MapPosition);

                if (card == tile.Building)
                    tile.Building = null;
                else
                    tile.Unit = null;

                card.MapPosition = HexMap.None;
            }
        }
    }
}
