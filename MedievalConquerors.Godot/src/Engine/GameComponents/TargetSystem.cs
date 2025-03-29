using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class TargetSystem : GameComponent, IAwake
{
    private HexMap _map;
    private EventAggregator _events;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        // TODO: Is this valid for technology cards, or is this validation only relevant for units/buildings?
        var tile = _map.GetTile(action.TargetTile);
        var validTiles = GetTargetCandidates(action.CardToPlay);

        if (!validTiles.Contains(tile.Position))
            validator.Invalidate("Invalid target tile for card.");
    }

    public List<Vector2I> GetTargetCandidates(Card card)
    {
        var player = card.Owner;
        var spawnPoint = card.GetAttribute<SpawnPointAttribute>();

        if(spawnPoint == null)
            return [];

        if (spawnPoint.SpawnTags == Tags.TownCenter)
            return _map.GetReachable(player.TownCenter.Position, spawnPoint.SpawnRange == 0 ? player.InfluenceRange : spawnPoint.SpawnRange).ToList();

        var targetCandidates = new List<Vector2I>();
        var buildings = player.Map.Where(c => c.CardData.CardType == CardType.Building);
        foreach (var building in buildings)
        {
            if (building.CardData.Tags.HasFlag(spawnPoint.SpawnTags))
            {
                // TODO: We may want to pull the garrison check into an extension method, as well as other attribute checks
                //       This is so we can just call `building.CanGarrison(source)` directly instead of using `GetAttribute`
                if (spawnPoint.SpawnRange == 0 && building.GetAttribute<GarrisonCapacityAttribute>()?.CanGarrison(card) == true)
                {
                    targetCandidates.Add(building.MapPosition);
                }
                else
                {
                    var surroundingNeighbors = _map.GetReachable(building.MapPosition, spawnPoint.SpawnRange);
                    targetCandidates.AddRange(surroundingNeighbors);
                }
            }
        }

        return targetCandidates;
    }
}
