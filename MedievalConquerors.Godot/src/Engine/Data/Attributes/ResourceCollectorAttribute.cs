namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceCollectorAttribute : CardAttribute
{
    public ResourceType Resource { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
