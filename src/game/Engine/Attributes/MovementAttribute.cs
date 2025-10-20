using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record MovementAttribute : ICardAttribute
{
    public int Distance { get; init; }
}
