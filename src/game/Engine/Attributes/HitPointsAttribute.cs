using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record HitPointsAttribute : CardAttribute
{
    public int Health { get; set; }
}
