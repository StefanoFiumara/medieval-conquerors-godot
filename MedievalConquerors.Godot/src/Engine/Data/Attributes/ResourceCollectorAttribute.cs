namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceCollectorAttribute : ICardAttribute
{
    public ResourceType Resource { get; set; }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}
