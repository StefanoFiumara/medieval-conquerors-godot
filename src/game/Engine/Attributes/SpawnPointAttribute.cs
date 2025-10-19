using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record SpawnPointAttribute : ICardAttribute
{
    public Tags SpawnTags { get; init; }
    public int SpawnRange { get; init; }
}
