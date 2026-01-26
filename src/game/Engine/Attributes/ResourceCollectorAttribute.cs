using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record ResourceCollectorAttribute : CardAttribute
{
    public ResourceType Resource { get; init; }
}
