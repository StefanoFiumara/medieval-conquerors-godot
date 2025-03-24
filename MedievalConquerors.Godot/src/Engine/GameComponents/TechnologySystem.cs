using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class TechnologySystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private AbilitySystem _abilitySystem;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _abilitySystem = Game.GetComponent<AbilitySystem>();

        // TODO: validate that when playing a card with type technology, we have an ability attached we can trigger
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<ResearchTechnologyAction>(GameEvent.Prepare<ResearchTechnologyAction>(), OnPrepareResearchTechnology);
        // TODO: On perform research technology, we should simply send the card to the banished pile, since it shouldn't come back again.
        //      We will have already reacted with the ability trigger in the prepare step.
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
        var ability = action.Card.GetAttribute<AbilityAttribute>();
        if (ability != null)
        {
            // TODO: OR we can just react with Ability Action here?
            _abilitySystem.TriggerAbility(action.Card);
        }
    }
}
