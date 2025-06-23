using LiteDB;
using MedievalConquerors.Engine.Data;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Attributes;

public class MovementAttribute : CardAttribute
{
    public int Distance { get; set; }

    // TODO: Do we need an additional attribute to ignore this properly in the attribute editor?
    //      Alternatively - we can just check for BsonIgnore when choosing which props to display
    [BsonIgnore] [MapperIgnore]
    public int RemainingDistance { get; set; }

    public bool CanMove(int amount) => RemainingDistance >= amount;
    public void Move(int amount) => RemainingDistance -= amount;

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
