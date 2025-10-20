using System;
using System.Collections.Generic;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

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
        var garrisonAttribute = action.Building.GetAttribute<GarrisonCapacityAttribute>();

        if (garrisonAttribute == null)
        {
            validator.Invalidate("This building cannot garrison any units.");
            return;
        }

        if (!_tracker.CanGarrison(action.Building, action.Unit))
        {
            validator.Invalidate("This building cannot garrison the given unit.");
            return;
        }

        var currentCount = _tracker.GetGarrisonCount(action.Building);

        if (currentCount >= garrisonAttribute.Limit)
            validator.Invalidate($"Garrison capacity reached ({garrisonAttribute.Limit}).");
    }

    private void OnPerformGarrison(GarrisonAction action)
    {
        _tracker.Garrison(action.Building, action.Unit);
    }

    public bool CanGarrison(Card building, Card unit) => _tracker.CanGarrison(building, unit);
    public IReadOnlyList<Card> GetGarrisonedUnits(Card building) => _tracker.GetGarrisonedUnits(building);
}

public class GarrisonTracker : GameComponent
{
    private readonly Dictionary<Card, List<Card>> _garrisonedUnits = new();

    public bool CanGarrison(Card building, Card unit)
    {
        if (!building.HasAttribute<GarrisonCapacityAttribute>(out var capacity))
            return false;

        var currentGarrison = GetGarrisonCount(building);
        var isEconomicUnit = unit.Data.Tags.HasFlag(Tags.Economic) && unit.Data.CardType == CardType.Unit;
        var buildingHasRoom = currentGarrison < capacity.Limit;

        return isEconomicUnit && buildingHasRoom;
    }

    public void Garrison(Card building, Card unit)
    {
        var garrisonAttribute = building.GetAttribute<GarrisonCapacityAttribute>();

        if (garrisonAttribute == null)
            throw new InvalidOperationException("Building does not have a GarrisonCapacityAttribute.");

        if (!_garrisonedUnits.ContainsKey(building))
            _garrisonedUnits[building] = new();

        _garrisonedUnits[building].Add(unit);
    }

    public void Ungarrison(Card building, Card unit)
    {
        if (_garrisonedUnits.TryGetValue(building, out var units))
            units.Remove(unit);
    }

    public IReadOnlyList<Card> GetGarrisonedUnits(Card building)
    {
        return _garrisonedUnits.TryGetValue(building, out var units)
            ? units.AsReadOnly()
            : new List<Card>().AsReadOnly();
    }

    public int GetGarrisonCount(Card building)
    {
        return _garrisonedUnits.TryGetValue(building, out var units) ? units.Count : 0;
    }

    public void Clear(Card building)
    {
        _garrisonedUnits.Remove(building);
    }
}
