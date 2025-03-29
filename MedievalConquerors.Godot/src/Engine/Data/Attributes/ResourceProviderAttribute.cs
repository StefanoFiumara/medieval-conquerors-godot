namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceProviderAttribute : CardAttribute
{
    public ResourceType Resource { get; set; }
    public int ResourceYield { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
