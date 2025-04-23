using LiteDB;
using MedievalConquerors.Engine.Data;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Attributes;

public class MovementAttribute : CardAttribute
{
    public int Distance { get; set; }

    [BsonIgnore] [MapperIgnore]
    public int RemainingDistance { get; set; }

    public bool CanMove(int amount) => RemainingDistance >= amount;
    public void Move(int amount) => RemainingDistance -= amount;

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
