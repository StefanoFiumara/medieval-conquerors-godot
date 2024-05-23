using System;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.Actions;

public class GameAction
{
    public Guid Id { get; }
    public bool IsCanceled { get; private set; }

    public ActionPhase PreparePhase { get; }
    public ActionPhase PerformPhase { get; }
    public ActionPhase CancelPhase { get; }

    public int Priority { get; set; }
    public int OrderOfPlay { get; set; }

    
    // TODO: Possible to set SourcePlayer from ctor? What about when using IAbilityLoader?
    // TODO: Should we just store the source player ID so it can be grabbed from the match instead?
    //       This may be useful for networking later.
    /// <summary>
    /// The player that initiated this GameAction
    /// </summary>
    public IPlayer SourcePlayer { get; set; }

    /// <summary>
    /// The card that initiated this GameAction
    /// </summary>
    public Card SourceCard { get; set; }

    public GameAction()
    {
        Id = Guid.NewGuid(); // Global.GenerateId(GetType());
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