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
            
            var tilesToCollect =
                _map.GetNeighbors(card.MapPosition)
                    .Where(t => collectorAttribute.Resource == t.ResourceType)
                    .Where(t => t.ResourceYield > 0);

            int collected = 0;
            foreach (var tile in tilesToCollect)
            {
                player.Resources[collectorAttribute.Resource] += Mathf.CeilToInt(tile.ResourceYield * collectorAttribute.GatherRate);
                collected++;

                if (collected >= garrisonAttribute.Units.Count)
                    break;
            }
        }
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        var resourceCost = action.CardToPlay.GetAttribute<ResourceCostAttribute>();
        if (resourceCost == null)
            return;
		
        if(!action.SourcePlayer.Resources.CanAfford(resourceCost))
            validator.Invalidate("Not enough resource to play card.");
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        var resourceCost = action.CardToPlay.GetAttribute<ResourceCostAttribute>();
        if (resourceCost != null)
            action.SourcePlayer.Resources.Subtract(resourceCost);

        var resourceCollector = action.CardToPlay.GetAttribute<ResourceCollectorAttribute>();
        if (resourceCollector != null) 
            action.SourcePlayer.Resources.IncreaseLimit(resourceCollector.StorageLimitIncrease);
    }
}