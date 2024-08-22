namespace MedievalConquerors.Engine.Data.Attributes;

public class SpawnPointAttribute : ICardAttribute
{
    public Tags SpawnTags { get; set; }
    public int SpawnRange { get; set; }
    
    public void OnTurnStart() { }

    public ICardAttribute Clone()
    {
        return AttributeMapper.Clone(this);
    }
}