using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Data.Attributes;

[Mapper(UseDeepCloning = true)]
public static partial class AttributeMapper
{
    [MapperIgnoreTarget(nameof(MovementAttribute.RemainingDistance))]
    [MapperIgnoreSource(nameof(MovementAttribute.RemainingDistance))]
    public static partial MovementAttribute Clone(this MovementAttribute source);
    public static partial ResourceCostAttribute Clone(this ResourceCostAttribute source);

    [MapperIgnoreTarget(nameof(GarrisonCapacityAttribute.Units))]
    [MapperIgnoreSource(nameof(GarrisonCapacityAttribute.Units))]
    public static partial GarrisonCapacityAttribute Clone(this GarrisonCapacityAttribute source);

    public static partial HitPointsAttribute Clone(this HitPointsAttribute source);
    public static partial ResourceCollectorAttribute Clone(this ResourceCollectorAttribute source);

    public static partial ResourceProviderAttribute Clone(this ResourceProviderAttribute source);

    public static partial SpawnPointAttribute Clone(this SpawnPointAttribute source);

    public static partial AbilityAttribute Clone(this AbilityAttribute source);
}
