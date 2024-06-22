namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceCostAttribute : ICardAttribute
{
    public float Food { get; set; }
    public float Wood { get; set; }
    public float Gold { get; set; }
    public float Stone { get; set; }

    public void Reset() { }
    
    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}