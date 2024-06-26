namespace MedievalConquerors.Engine.Data.Attributes;

public class SpawnPointAttribute : ICardAttribute
{
    public Tags SpawnTags { get; set; }
    public int SpawnRange { get; set; }
    
    public void Reset() { }

    public ICardAttribute Clone()
    {
        return AttributeMapper.Clone(this);
    }
}