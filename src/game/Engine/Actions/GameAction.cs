using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.Actions;

// TODO: Do we need to extract an interface for a game action?
public class GameAction
{
    public Guid Id { get; }
    public bool IsCanceled { get; private set; }

    public ActionPhase PreparePhase { get; }
    public ActionPhase PerformPhase { get; }
    public ActionPhase CancelPhase { get; }

    public int Priority { get; set; }
    public int OrderOfPlay { get; set; }

    protected GameAction()
    {
        Id = Guid.NewGuid();
        PreparePhase = new ActionPhase(this, OnPrepare);
        PerformPhase = new ActionPhase(this, OnPerform);
        CancelPhase = new ActionPhase(this, OnCancel);
    }

    public void Cancel()
    {
        IsCanceled = true;
    }

    public ActionValidatorResult Validate(IGame gameState)
    {
        var validator = new ActionValidatorResult();

        var events = gameState.GetComponent<EventAggregator>();
        var eventName = GameEvent.Validate(GetType());

        events.Publish(eventName, this, validator);

        return validator;
    }

    private void OnPrepare(IGame gameState)
    {
        var events = gameState.GetComponent<EventAggregator>();
        var eventName = GameEvent.Prepare(GetType());
        events.Publish(eventName, this);
    }

    private void OnPerform(IGame gameState)
    {
        var events = gameState.GetComponent<EventAggregator>();
        var eventName = GameEvent.Perform(GetType());
        events.Publish(eventName, this);
    }

    private void OnCancel(IGame gameState)
    {
        var events = gameState.GetComponent<EventAggregator>();
        var eventName = GameEvent.Cancel(GetType());
        events.Publish(eventName, this);
    }

    public override string ToString() => GetType().Name;
}

public interface IAbilityLoader
{
    static abstract Dictionary<string, Type> GetParameters();
    void Load(IGame game, Card card, AbilityAttribute ability, ActionDefinition data, Vector2I targetTile);
}

public static class AbilityExtensions
{
    public static int ResolvePlayerTarget(this IAbilityLoader loader, IGame game, Card card, PlayerTarget target)
    {
        var player = card.Owner;
        var enemyPlayer = game.GetComponent<Match>().Players[1 - player.Id];

        return target switch
        {
            PlayerTarget.Owner => player.Id,
            PlayerTarget.Enemy => enemyPlayer.Id,
            _ => throw new ArgumentException("Player target is not set correctly")
        };
    }
}
