namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceCollectorAttribute : ICardAttribute
{
    public ResourceType Resource { get; set; }
    public float GatherRate { get; set; }
    public int StorageLimitIncrease { get; set; }
    
    public void OnTurnStart() { } 

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}