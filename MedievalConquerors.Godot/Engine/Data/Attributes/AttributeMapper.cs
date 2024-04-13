using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Data.Attributes;

[Mapper(UseDeepCloning = true)]
public static partial class AttributeMapper
{
    public static partial MovementAttribute Clone(this MovementAttribute source);
    public static partial ResourceCostAttribute Clone(this ResourceCostAttribute source);
}