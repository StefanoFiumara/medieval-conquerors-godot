using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record GarrisonCapacityAttribute : ICardAttribute
{
    public int Limit { get; init; }
}
