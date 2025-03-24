namespace MedievalConquerors.Engine.Data.Attributes;

public class SpawnPointAttribute : ICardAttribute
{
    public Tags SpawnTags { get; set; }
    public int SpawnRange { get; set; }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}
