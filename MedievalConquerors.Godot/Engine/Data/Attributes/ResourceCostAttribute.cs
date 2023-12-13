namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceCostAttribute : ICardAttribute
{
    public int Food { get; set; }
    public int Wood { get; set; }
    public int Gold { get; set; }
    public int Stone { get; set; }

    public void Reset()
    {
        // TODO: do we need to keep track of any cost changes here?
    }
    
    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}