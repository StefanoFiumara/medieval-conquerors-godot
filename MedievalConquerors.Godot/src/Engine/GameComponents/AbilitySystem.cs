using System;
using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
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
        int actionPriority = 99;
        foreach (var actionDef in action.Ability.Actions)
        {
            var type = Type.GetType(actionDef.ActionType);
            if (type == null)
            {
                _logger.Error($"Ability System could not find action name: {actionDef.ActionType}");
                return;
            }

            // TODO: get args from actionDef before creating instance.
            //      So we can use the default constructor without having to define a parameterless one.
            var ctor = type.GetConstructors().Single();
            var parameters = ctor.GetParameters().OrderBy(p => p.Position);
            var args = new List<object>();
            foreach (var parameter in parameters)
            {
                // TODO: use actionDef to populate args
            }

            if (Activator.CreateInstance(type, args) is not GameAction reaction)
            {
                _logger.Error($"Could not create instance of type {type.Name}");
                return;
            }

            reaction.Priority = actionPriority--;
            Game.AddReaction(reaction);
        }
    }

    public void TriggerAbility(Card card, AbilityAttribute ability)
    {
        var action = new AbilityAction(ability);

        if (Game.GetComponent<ActionSystem>().IsActive)
            Game.AddReaction(action);
        else
            Game.Perform(action);
    }
}
