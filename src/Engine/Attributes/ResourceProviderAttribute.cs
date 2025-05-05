using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public class ResourceProviderAttribute : CardAttribute
{
    public ResourceType Resource { get; set; }
    public int ResourceYield { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
