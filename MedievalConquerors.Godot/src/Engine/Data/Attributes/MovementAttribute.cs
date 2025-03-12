using LiteDB;

namespace MedievalConquerors.Engine.Data.Attributes;

public class MovementAttribute : ICardAttribute
{
    public int Distance { get; set; }

    [BsonIgnore]
    public int RemainingDistance { get; set; }

    public bool CanMove(int amount) => RemainingDistance >= amount;
    public void Move(int amount) => RemainingDistance -= amount;

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}
