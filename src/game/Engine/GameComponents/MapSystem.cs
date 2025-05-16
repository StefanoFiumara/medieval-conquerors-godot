using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class MapSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private HexMap _map;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();

        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);

        _events.Subscribe<SpawnUnitAction>(GameEvent.Perform<SpawnUnitAction>(), OnPerformSpawnUnit);
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        if(_map.GetTile(action.TargetTile) == null)
            validator.Invalidate("Invalid target tile for card.");
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        if (action.CardToPlay.CardData.CardType == CardType.Building)
            Game.AddReaction(new BuildStructureAction(action.CardToPlay, action.TargetTile));

        else if (action.CardToPlay.CardData.CardType == CardType.Unit)
        {
            var tile = _map.GetTile(action.TargetTile);

            if (tile.Building != null)
                Game.AddReaction(new GarrisonAction(action.CardToPlay, tile.Building));
            else
                Game.AddReaction(new SpawnUnitAction(action.CardToPlay, action.TargetTile));
        }
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

    private void OnPerformSpawnUnit(SpawnUnitAction action)
    {
        var tile = _map.GetTile(action.TargetTile);
        var player = action.UnitToSpawn.Owner;

        tile.Unit = action.UnitToSpawn;
        action.UnitToSpawn.MapPosition = tile.Position;
        player.MoveCard(action.UnitToSpawn, Zone.Map);
    }
}
