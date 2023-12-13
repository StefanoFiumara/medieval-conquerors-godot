namespace MedievalConquerors.Engine.Data.Attributes;

public class MoveAttribute : ICardAttribute
{
    public int Distance { get; set; }
    public int DistanceRemaining { get; set; }

    public MoveAttribute(int distance)
    {
        Distance = distance;
        DistanceRemaining = distance;
    }

    public void Reset()
    {
        DistanceRemaining = Distance;
    }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}