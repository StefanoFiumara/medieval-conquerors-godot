using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

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

        foreach (var card in player.Map)
        {
            var collectorAttribute = card.GetAttribute<ResourceCollectorAttribute>();
            var garrisonAttribute = card.GetAttribute<GarrisonCapacityAttribute>();

            if (card.CardData.CardType != CardType.Building) continue;
            if (collectorAttribute == null || garrisonAttribute == null) continue;
            if (garrisonAttribute.Units.Count == 0) continue;

            var resourceTiles = _map.GetNeighbors(card.MapPosition)
                .Where(t => collectorAttribute.Resource.HasFlag(t.ResourceType))
                .Where(t => t.ResourceYield > 0)
                .ToList();

            float efficiencyFactor = CalculateEfficiencyFactor(garrisonAttribute.Units.Count, resourceTiles.Count);
            int remainingVillagers = garrisonAttribute.Units.Count;

            while (remainingVillagers > 0)
            {
                var tile = resourceTiles[remainingVillagers % resourceTiles.Count];
                int amountCollected = Mathf.CeilToInt(tile.ResourceYield * efficiencyFactor);

                player.Resources[tile.ResourceType] += amountCollected;
                action.ResourcesCollected.Add((tile.Position, tile.ResourceType, amountCollected));

                remainingVillagers--;
            }
        }
    }

    private float CalculateEfficiencyFactor(int totalVillagers, int totalTiles)
    {
        if (totalTiles >= totalVillagers) return 1f;

        int extraVillagers = totalVillagers - totalTiles;
        var efficiency = 1.0f;
        const float decayFactor = 0.65f;

        for (int i = 0; i < extraVillagers; i++)
            efficiency *= decayFactor;

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
