using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public class ResourceCostAttribute : CardAttribute
{
    public int Food { get; set; }
    public int Wood { get; set; }
    public int Gold { get; set; }
    public int Stone { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
