namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceProviderAttribute : ICardAttribute
{
    public ResourceType Resource { get; set; }
    public int ResourceYield { get; set; }

    public void OnTurnStart() { }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}
