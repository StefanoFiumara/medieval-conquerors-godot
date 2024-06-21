using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Data.Attributes;

[Mapper(UseDeepCloning = true)]
public static partial class AttributeMapper
{
    public static partial MovementAttribute Clone(this MovementAttribute source);
    public static partial ResourceCostAttribute Clone(this ResourceCostAttribute source);
    
    [MapperIgnoreTarget(nameof(GarrisonAttribute.Units))]
    public static partial GarrisonAttribute Clone(this GarrisonAttribute source);

    public static partial HitPointsAttribute Clone(this HitPointsAttribute source);
}