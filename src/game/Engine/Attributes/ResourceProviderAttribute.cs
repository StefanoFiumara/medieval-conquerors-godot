using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record ResourceProviderAttribute : CardAttribute
{
    public ResourceType Resource { get; init; }
    public int Yield { get; init; }
}
