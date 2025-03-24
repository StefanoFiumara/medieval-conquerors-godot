using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class TechnologySystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private AbilitySystem _abilitySystem;
    private ILogger _logger;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _abilitySystem = Game.GetComponent<AbilitySystem>();
        _logger = Game.GetComponent<ILogger>();

        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<ResearchTechnologyAction>(GameEvent.Prepare<ResearchTechnologyAction>(), OnPrepareResearchTechnology);
        _events.Subscribe<ResearchTechnologyAction>(GameEvent.Perform<ResearchTechnologyAction>(), OnPerformResearchTechnology);
        // TODO: On perform research technology, we should simply send the card to the banished pile, since it shouldn't come back again.
        //      We will have already reacted with the ability trigger in the prepare step.
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        if (action.CardToPlay.CardData.CardType == CardType.Technology && action.CardToPlay.GetAttribute<AbilityAttribute>() == null)
        {
            _logger.Warn("Attempted to validate Technology Card without ability.");
            validator.Invalidate("Technology Card does not have ability attribute.");
        }
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        if (action.CardToPlay.CardData.CardType == CardType.Technology)
        {
            var researchAction = new ResearchTechnologyAction(action.CardToPlay);
            Game.AddReaction(researchAction);
        }
    }

    private void OnPrepareResearchTechnology(ResearchTechnologyAction action)
    {
        var ability = action.Card.GetAttribute<OnCardPlayedAbility>();
        if (ability != null)
            _abilitySystem.TriggerAbility(action.Card, ability);
    }

    private void OnPerformResearchTechnology(ResearchTechnologyAction action)
    {
        action.Card.Owner.MoveCard(action.Card, Zone.Banished);
    }
}
