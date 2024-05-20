using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this system
public class ResourceSystem : GameComponent, IAwake
{
    private EventAggregator _events;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
		
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
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
        if (resourceCost == null)
            return;
		
        action.SourcePlayer.Resources.Subtract(resourceCost);
    }
}