namespace MedievalConquerors.Engine.Data.Attributes;

public class MoveAttribute : ICardAttribute
{
    public int Distance { get; init; }
    public int DistanceRemaining { get; set; }

    public MoveAttribute()
    {
        
    }

    public void Reset()
    {
        DistanceRemaining = Distance;
    }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}