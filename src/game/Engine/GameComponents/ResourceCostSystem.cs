using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class ResourceCostSystem : GameComponent, IAwake
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