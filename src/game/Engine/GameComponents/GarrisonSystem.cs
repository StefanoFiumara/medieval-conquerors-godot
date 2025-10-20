using System.Collections.Generic;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this system
public class GarrisonSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private GarrisonTracker _tracker;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _tracker = new GarrisonTracker();

        _events.Subscribe<GarrisonAction>(GameEvent.Perform<GarrisonAction>(), OnPerformGarrison);
        _events.Subscribe<GarrisonAction, ActionValidatorResult>(GameEvent.Validate<GarrisonAction>(), OnValidateGarrison);
    }

    private void OnValidateGarrison(GarrisonAction action, ActionValidatorResult validator)
    {
        if (!action.Building.HasAttribute<GarrisonCapacityAttribute>(out var garrisonAttribute))
        {
            validator.Invalidate("This building cannot garrison any units.");
            return;
        }

        if (!CanGarrison(action.Building, action.Unit))
        {
            validator.Invalidate("This building cannot garrison the given unit.");
            return;
        }

        var currentCount = _tracker.GetGarrisonCount(action.Building);

        if (currentCount >= garrisonAttribute.Limit)
            validator.Invalidate($"Garrison capacity reached ({garrisonAttribute.Limit}).");
    }

    private void OnPerformGarrison(GarrisonAction action) => _tracker.Garrison(action.Building, action.Unit);

    public bool CanGarrison(Card building, Card unit) => _tracker.CanGarrison(building, unit);
    public IReadOnlyList<Card> GetGarrisonedUnits(Card building) => _tracker.GetGarrisonedUnits(building);
}
