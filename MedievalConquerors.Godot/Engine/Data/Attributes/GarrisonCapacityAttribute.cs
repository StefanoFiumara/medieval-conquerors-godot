using System.Collections.Generic;
using LiteDB;

namespace MedievalConquerors.Engine.Data.Attributes;

public class GarrisonCapacityAttribute : ICardAttribute
{
    public int Limit { get; set; }

    [BsonIgnore] 
    public List<Card> Units { get; private set; } = new();
    
    public bool CanGarrison(Card unit) => 
        Units.Count < Limit 
        && unit.CardData.Tags.HasFlag(Tags.Economic) 
        && unit.CardData.CardType == CardType.Unit;

    public void Garrison(Card unit)
    {
        if(CanGarrison(unit))
            Units.Add(unit);
    }
    
    public void Reset() { }

    public ICardAttribute Clone()
    {
        return AttributeMapper.Clone(this);
    }
}