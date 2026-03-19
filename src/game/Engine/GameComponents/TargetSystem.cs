using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class TargetSystem : GameComponent, IAwake
{
    private HexMap _map;
    private EventAggregator _events;
    private GarrisonSystem _garrisonSystem;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();
        _garrisonSystem = Game.GetComponent<GarrisonSystem>();

        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        // TODO: support cards without target tile
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

        if (spawnPoint.SpecificCardId != 0)
        {
            return player.Map.Where(c => c.Data.Id == spawnPoint.SpecificCardId)
                .Select(c => c.MapPosition)
                .ToList();
        }

        if (spawnPoint.Garrison && spawnPoint.SpawnTags != Tags.None)
        {
            return player.Map
                .Where(c => c.Data.CardType == CardType.Building)
                .Where(b => b.Data.Tags.HasFlag(spawnPoint.SpawnTags))
                .Where(b => _garrisonSystem.CanGarrison(b, card))
                .Select(b => b.MapPosition)
                .ToList();
        }

        var availableTiles = _map.GetReachable(player.TownCenter.Position, player.InfluenceRange);

        if (spawnPoint.SpawnTags != Tags.None)
        {
            var buildings = player.Map.Where(c => c.Data.Tags.HasFlag(spawnPoint.SpawnTags)).Select(b => b.MapPosition).ToList();
            availableTiles = availableTiles.Where(t => _map.GetNeighbors(t).Any(n => buildings.Contains(n.Position)));
        }

        if (spawnPoint.Resource != ResourceType.None)
        {
            availableTiles = availableTiles.Where(t => _map.GetNeighbors(t).Any(n =>
                n.ResourceType != ResourceType.None && spawnPoint.Resource.HasFlag(n.ResourceType)));
        }

        return availableTiles.ToList();
    }
}
