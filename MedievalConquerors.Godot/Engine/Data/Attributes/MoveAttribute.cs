using LiteDB;

namespace MedievalConquerors.Engine.Data.Attributes;

public class MoveAttribute : ICardAttribute
{
    private int _distance;

    public int Distance
    {
        get => _distance;
        set
        {
            _distance = value;
            RemainingDistance = value;
        }
    }
    
    [BsonIgnore]
    public int RemainingDistance { get; private set; }
    
    public bool CanMove(int amount) => RemainingDistance >= amount;
    public void Move(int amount) => RemainingDistance -= amount;

    public void Reset()
    {
        RemainingDistance = Distance;
    }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}