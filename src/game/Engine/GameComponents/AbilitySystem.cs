using System;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class AbilitySystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private ILogger _logger;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _logger = Game.GetComponent<ILogger>();

        _events.Subscribe<AbilityAction>(GameEvent.Perform<AbilityAction>(), OnPerformAbility);

        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);

        _events.Subscribe<PlayActionCardAction>(GameEvent.Perform<PlayActionCardAction>(), OnPerformPlayActionCard);
        _events.Subscribe<ResearchTechnologyAction>(GameEvent.Prepare<ResearchTechnologyAction>(), OnPrepareResearchTechnology);
        _events.Subscribe<ResearchTechnologyAction>(GameEvent.Perform<ResearchTechnologyAction>(), OnPerformResearchTechnology);
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        if (action.CardToPlay.Data.CardType is CardType.Action or CardType.Technology
            && !action.CardToPlay.HasAttribute<OnCardPlayedAbility>())
        {
            _logger.Warn($"Attempted to validate {action.CardToPlay.Data.CardType} Card without ability.");
            validator.Invalidate($"{action.CardToPlay.Data.CardType} Card does not have ability attribute.");
        }
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        if (action.CardToPlay.HasAttribute<OnCardPlayedAbility>())
        {

            switch (action.CardToPlay.Data.CardType)
            {
                case CardType.Action:
                {
                    var actionCardAction = new PlayActionCardAction(action.CardToPlay, action.TargetTile);
                    Game.AddReaction(actionCardAction);
                    break;
                }
                case CardType.Technology:
                {
                    var researchAction = new ResearchTechnologyAction(action.CardToPlay, action.TargetTile);
                    Game.AddReaction(researchAction);
                    break;
                }
                default:
                    TriggerAbility<OnCardPlayedAbility>(action.CardToPlay, action.TargetTile);
                    break;
            }
        }
    }

    // TODO: Trigger ability in perform, the react with discard/banish card actions

    private void OnPrepareResearchTechnology(ResearchTechnologyAction action)
    {
        TriggerAbility<OnCardPlayedAbility>(action.Card, action.TargetTile);
        Game.AddReaction(new BanishCardsAction([action.Card]));
    }

    private void OnPerformPlayActionCard(PlayActionCardAction action)
    {
        TriggerAbility<OnCardPlayedAbility>(action.Card, action.TargetTile);
        Game.AddReaction(new DiscardCardsAction([action.Card]));
    }

    private void OnPerformResearchTechnology(ResearchTechnologyAction action)
        => action.Card.Owner.MoveCard(action.Card, Zone.Banished);

    private void OnPerformAbility(AbilityAction action)
    {
        int actionPriority = 99;
        foreach (var actionDef in action.Ability.Actions)
        {

            var reaction = LoadAction(action.Card, action.Ability, actionDef, action.TargetTile);
            if (reaction != null)
            {
                reaction.Priority = actionPriority--;
                Game.AddReaction(reaction);
            }
        }
    }

    private GameAction LoadAction(Card card, AbilityAttribute ability, ActionDefinition actionDef, Vector2I targetTile)
    {
        var type = Type.GetType(actionDef.ActionType);
        if (type == null)
        {
            _logger.Error($"OnPerformAbility: could not find action name {actionDef.ActionType}");
            return null;
        }

        if (Activator.CreateInstance(type) is not GameAction action)
        {
            _logger.Error($"OnPerformAbility: Could not create instance of type {type.Name}");
            return null;
        }

        if (action is IAbilityLoader loadable)
        {
            loadable.Load(Game, card, ability, actionDef, targetTile);
        }
        else
        {
            _logger.Error($"OnPerformAbility: Loaded action {type.Name} does not implement ILoadableAction");
            return null;
        }

        return action;
    }

    private void TriggerAbility<TAbility>(Card card, Vector2I targetTile) where TAbility : AbilityAttribute
    {
        var ability = card.GetAttribute<TAbility>();
        var action = new AbilityAction(card, ability, targetTile);

        if (Game.GetComponent<ActionSystem>().IsActive)
            Game.AddReaction(action);
        else
            Game.Perform(action);
    }
}
