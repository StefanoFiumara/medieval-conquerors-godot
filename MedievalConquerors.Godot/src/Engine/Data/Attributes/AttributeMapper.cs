using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Data.Attributes;

[Mapper(UseDeepCloning = true)]
public static partial class AttributeMapper
{
    public static partial MovementAttribute Clone(this MovementAttribute source);
    public static partial ResourceCostAttribute Clone(this ResourceCostAttribute source);

    public static partial GarrisonCapacityAttribute Clone(this GarrisonCapacityAttribute source);

    public static partial HitPointsAttribute Clone(this HitPointsAttribute source);
    public static partial ResourceCollectorAttribute Clone(this ResourceCollectorAttribute source);

    public static partial ResourceProviderAttribute Clone(this ResourceProviderAttribute source);

    public static partial SpawnPointAttribute Clone(this SpawnPointAttribute source);

    public static partial OnCardPlayedAbility Clone(this OnCardPlayedAbility source);
    public static partial OnCardActivatedAbility Clone(this OnCardActivatedAbility source);
}
