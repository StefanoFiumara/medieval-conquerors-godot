namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceCollectorAttribute : ICardAttribute
{
    public ResourceType Resource { get; set; }
    public float GatherRate { get; set; }
    public float StorageLimitIncrease { get; set; }
    
    public void Reset() { } 

    public ICardAttribute Clone()
    {
        return AttributeMapper.Clone(this);
    }
}