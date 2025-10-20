using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record MovementAttribute : ICardAttribute
{
    public int Distance { get; init; }
    public bool CanMove(int amount) => Distance >= amount;
}

public record MovementModifier : Modifier<MovementAttribute>
{
    public int DistanceTraveled { get; init; }

    public override MovementAttribute Apply(MovementAttribute original)
        => original with { Distance = original.Distance - DistanceTraveled };
}
