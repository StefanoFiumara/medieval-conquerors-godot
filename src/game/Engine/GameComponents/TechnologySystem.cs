using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;

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
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        if (action.CardToPlay.Data.CardType == CardType.Technology && action.CardToPlay.GetAttribute<OnCardPlayedAbility>() == null)
        {
            _logger.Warn("Attempted to validate Technology Card without ability.");
            validator.Invalidate("Technology Card does not have ability attribute.");
        }
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        if (action.CardToPlay.Data.CardType == CardType.Technology)
        {
            var researchAction = new ResearchTechnologyAction(action.CardToPlay, action.TargetTile);
            Game.AddReaction(researchAction);
        }
    }

    private void OnPrepareResearchTechnology(ResearchTechnologyAction action)
    {
        if (action.Card.HasAttribute<OnCardPlayedAbility>(out var ability))
            _abilitySystem.TriggerAbility(action.Card, ability, action.TargetTile);
    }

    private void OnPerformResearchTechnology(ResearchTechnologyAction action)
    {
        action.Card.Owner.MoveCard(action.Card, Zone.Banished);
    }
}
