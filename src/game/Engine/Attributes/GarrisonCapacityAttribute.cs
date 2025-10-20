using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record GarrisonCapacityAttribute : ICardAttribute
{
    public int Limit { get; init; }

    // TODO: Reimplement this logic in the modifier system
    // [BsonIgnore] [MapperIgnore]
    // public List<Card> Units { get; } = new();

    // public void Garrison(Card unit)
    // {
    //     Units.Add(unit);
    // }

    public static bool CanGarrison(Card unit) =>
        unit.Data.Tags.HasFlag(Tags.Economic)
     // && Units.Count < Limit
        && unit.Data.CardType == CardType.Unit;
}
