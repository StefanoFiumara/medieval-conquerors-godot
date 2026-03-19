using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record SpawnPointAttribute : CardAttribute
{
    public bool Garrison { get; init; }
    public Tags SpawnTags { get; init; }
    public ResourceType Resource { get; init; }
    public int SpecificCardId { get; init; }
}
