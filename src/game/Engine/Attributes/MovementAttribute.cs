using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record MovementAttribute : CardAttribute
{
    public int Distance { get; init; }
}
