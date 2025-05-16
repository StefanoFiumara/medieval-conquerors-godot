using System;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;

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
    }

    private void OnPerformAbility(AbilityAction action)
    {
        // TODO: Should this priority value be hard coded?
        // TODO: Unit tests
        int actionPriority = 99;
        foreach (var actionDef in action.Ability.Actions)
        {

            var reaction = LoadAction(action.Ability, actionDef, action.TargetTile);
            if (reaction != null)
            {
                reaction.Priority = actionPriority--;
                Game.AddReaction(reaction);
            }
        }
    }

    private GameAction LoadAction(AbilityAttribute ability, ActionDefinition actionDef, Vector2I targetTile)
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
            loadable.Load(Game, ability, actionDef, targetTile);
        }
        else
        {
            _logger.Error($"OnPerformAbility: Loaded action {type.Name} does not implement ILoadableAction");
            return null;
        }

        return action;
    }

    public void TriggerAbility(AbilityAttribute ability, Vector2I targetTile)
    {
        var action = new AbilityAction(ability, targetTile);

        if (Game.GetComponent<ActionSystem>().IsActive)
            Game.AddReaction(action);
        else
            Game.Perform(action);
    }
}
