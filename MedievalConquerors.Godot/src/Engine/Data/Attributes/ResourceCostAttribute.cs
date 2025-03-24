namespace MedievalConquerors.Engine.Data.Attributes;

public class ResourceCostAttribute : ICardAttribute
{
    public int Food { get; set; }
    public int Wood { get; set; }
    public int Gold { get; set; }
    public int Stone { get; set; }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}
