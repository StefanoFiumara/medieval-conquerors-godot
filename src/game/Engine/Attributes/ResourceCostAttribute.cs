using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record ResourceCostAttribute : CardAttribute
{
    public int Food { get; init; }
    public int Wood { get; init; }
    public int Gold { get; init; }
    public int Stone { get; init; }
}
