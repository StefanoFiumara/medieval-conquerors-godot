using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public class SpawnPointAttribute : CardAttribute
{
    public Tags SpawnTags { get; set; }
    public int SpawnRange { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
