using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class ActionCardSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private ILogger _logger;
    private AbilitySystem _abilitySystem;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _abilitySystem = Game.GetComponent<AbilitySystem>();
        _logger = Game.GetComponent<ILogger>();

        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);

        _events.Subscribe<PlayActionCardAction>(GameEvent.Prepare<PlayActionCardAction>(), OnPreparePlayActionCard);
        _events.Subscribe<PlayActionCardAction>(GameEvent.Perform<PlayActionCardAction>(), OnPerformPlayActionCard);
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        if (action.CardToPlay.Data.CardType == CardType.Action && action.CardToPlay.GetAttribute<OnCardPlayedAbility>() == null)
        {
            _logger.Warn("Attempted to validate Action Card without ability.");
            validator.Invalidate("Action Card does not have ability attribute.");
        }
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        if (action.CardToPlay.Data.CardType == CardType.Action)
        {
            var actionCardAction = new PlayActionCardAction(action.CardToPlay, action.TargetTile);
            Game.AddReaction(actionCardAction);
        }
    }

    private void OnPreparePlayActionCard(PlayActionCardAction action)
    {
        if (action.Card.HasAttribute<OnCardPlayedAbility>(out var ability))
            _abilitySystem.TriggerAbility(action.Card, ability, action.TargetTile);
    }

    private void OnPerformPlayActionCard(PlayActionCardAction action)
    {
        action.Card.Owner.MoveCard(action.Card, Zone.Discard);
    }
}
