using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record SpawnPointAttribute : CardAttribute
{
    public Tags SpawnTags { get; init; }
    public int SpawnRange { get; init; }
}
