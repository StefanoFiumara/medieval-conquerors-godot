using LiteDB;

namespace MedievalConquerors.Engine.Data.Attributes;

public class MovementAttribute : ICardAttribute
{
    private int _distance;
    public int Distance
    {
        get => _distance;
        set
        {
            _distance = value;
            // TODO: May not need this, remaining distance can default to zero, and can be set with Reset() at the beginning of the player's turn.
            //       This gives us the "Summoning Sickness" mechanic for free.
            //       If this is implemented, we will have to update unit tests accordingly.
            RemainingDistance = value;
        }
    }
    
    [BsonIgnore]
    public int RemainingDistance { get; private set; }
    
    public bool CanMove(int amount) => RemainingDistance >= amount;
    public void Move(int amount) => RemainingDistance -= amount;

    public void OnTurnStart()
    {
        RemainingDistance = Distance;
    }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}