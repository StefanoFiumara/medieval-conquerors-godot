using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record GarrisonCapacityAttribute : CardAttribute
{
    public int Limit { get; init; }
}
