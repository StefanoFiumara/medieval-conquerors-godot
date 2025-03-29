namespace MedievalConquerors.Engine.Data.Attributes;

public class SpawnPointAttribute : CardAttribute
{
    public Tags SpawnTags { get; set; }
    public int SpawnRange { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
