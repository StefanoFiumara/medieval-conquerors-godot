using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public class ResourceCollectorAttribute : CardAttribute
{
    public ResourceType Resource { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
