using System.Collections.Generic;
using LiteDB;
using MedievalConquerors.Engine.Data;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Attributes;

public class GarrisonCapacityAttribute : CardAttribute
{
    public int Limit { get; set; }

    [BsonIgnore] [MapperIgnore]
    public List<Card> Units { get; private set; } = new();

    public bool CanGarrison(Card unit) =>
        Units.Count < Limit
        && unit.CardData.Tags.HasFlag(Tags.Economic)
        && unit.CardData.CardType == CardType.Unit;

    public void Garrison(Card unit)
    {
        Units.Add(unit);
    }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
