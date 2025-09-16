using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this system
public class ResourceSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private Match _match;
    private HexMap _map;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<HexMap>();
        _events = Game.GetComponent<EventAggregator>();

        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);

        _events.Subscribe<CollectResourcesAction>(GameEvent.Perform<CollectResourcesAction>(), OnPerformCollectResources);
    }

    private void OnPerformCollectResources(CollectResourcesAction action)
    {
        var player = _match.Players[action.TargetPlayerId];

        // NOTE: Track which resource collector has collected which resource tile.
        //       This helps us prevent double dipping when a resource tile is adjacent to two different collectors.
        var resourceOwnership = new Dictionary<Vector2I, Vector2I>();

        foreach (var card in player.Map)
        {
            var collectorAttribute = card.GetAttribute<ResourceCollectorAttribute>();
            var garrisonAttribute = card.GetAttribute<GarrisonCapacityAttribute>();

            // only process garrisoned buildings with a ResourceCollector attribute
            if (card.Data.CardType != CardType.Building) continue;
            if (collectorAttribute == null || garrisonAttribute == null) continue;
            if (garrisonAttribute.Units.Count == 0) continue;

            var resourcesToCollect = CalculateYields(collectorAttribute, _map.GetNeighbors(card.MapPosition))
                // Filter out tiles that have been already collected by other collectors
                .Where(t => !resourceOwnership.ContainsKey(t.Position) || resourceOwnership[t.Position] == card.MapPosition)
                .ToList();

            float efficiencyFactor = CalculateEfficiency(garrisonAttribute.Units.Count, resourcesToCollect.Count);

            int remainingVillagers = garrisonAttribute.Units.Count;
            while (remainingVillagers > 0 && resourcesToCollect.Count > 0)
            {
                var tile = resourcesToCollect[remainingVillagers % resourcesToCollect.Count];
                int amountCollected = Mathf.CeilToInt(tile.Yield * efficiencyFactor);

                player.Resources[tile.Resource] += amountCollected;
                action.ResourcesCollected.Add((tile.Position, tile.Resource, amountCollected));

                remainingVillagers--;

                resourceOwnership[tile.Position] = card.MapPosition;
            }
        }
    }

    private IEnumerable<(Vector2I Position, ResourceType Resource, int Yield)> CalculateYields(ResourceCollectorAttribute collector, IEnumerable<TileData> tiles)
    {
        foreach (var tile in tiles)
        {
            // Determine if this tile has a resource to collect by checking if it has a specific resource type
            // and if not, check if it contains a building with a ResourceProviderAttribute
            if (collector.Resource.HasFlag(tile.ResourceType) && tile.ResourceYield > 0)
            {
                yield return (tile.Position, tile.ResourceType, tile.ResourceYield);
            }
            else
            {
                var provider = tile.Building?.GetAttribute<ResourceProviderAttribute>();
                if (provider != null && collector.Resource.HasFlag(provider.Resource))
                {
                    yield return (tile.Position, provider.Resource, provider.ResourceYield);
                }
            }
        }
    }

    private float CalculateEfficiency(int totalVillagers, int totalTiles)
    {
        // Diminishing returns formula used when multiple villagers are gathering from the same tile
        if (totalTiles >= totalVillagers) return 1f;

        int extraVillagers = totalVillagers - totalTiles;
        var efficiency = 1.0f;
        const float DECAY_FACTOR = 0.65f;

        for (int i = 0; i < extraVillagers; i++)
            efficiency *= DECAY_FACTOR;

        return efficiency;
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        var resourceCost = action.CardToPlay.GetAttribute<ResourceCostAttribute>();
        if (resourceCost == null)
            return;

        var player = action.CardToPlay.Owner;
        if(!player.Resources.CanAfford(resourceCost))
            validator.Invalidate("Not enough resource to play card.");
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        var resourceCost = action.CardToPlay.GetAttribute<ResourceCostAttribute>();
        if (resourceCost != null)
        {
            var player = action.CardToPlay.Owner;
            player.Resources.Subtract(resourceCost);
        }
    }
}
