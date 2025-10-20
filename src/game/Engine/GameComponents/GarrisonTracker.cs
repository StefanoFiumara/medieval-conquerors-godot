using System;
using System.Collections.Generic;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.GameComponents;

public class GarrisonTracker
{
    private readonly Dictionary<Card, List<Card>> _garrisonedUnits = new();

    public bool CanGarrison(Card building, Card unit)
    {
        if (building.Data.CardType != CardType.Building)
            return false;

        if (!building.HasAttribute<GarrisonCapacityAttribute>(out var garrisonAttribute))
            return false;

        var currentCount = GetGarrisonCount(building);
        var isEconomicUnit = unit.Data.Tags.HasFlag(Tags.Economic) && unit.Data.CardType == CardType.Unit;
        var buildingHasCapacity = currentCount < garrisonAttribute.Limit;

        return isEconomicUnit && buildingHasCapacity;
    }

    public void Garrison(Card building, Card unit)
    {
        var garrisonAttribute = building.GetAttribute<GarrisonCapacityAttribute>();

        if (garrisonAttribute == null)
            throw new InvalidOperationException("Building does not have a GarrisonCapacityAttribute.");

        if (!_garrisonedUnits.ContainsKey(building))
            _garrisonedUnits[building] = [];

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
            : [];
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
