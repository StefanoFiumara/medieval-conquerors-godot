using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record ResourceCollectorAttribute : ICardAttribute
{
    public ResourceType Resource { get; init; }
}
