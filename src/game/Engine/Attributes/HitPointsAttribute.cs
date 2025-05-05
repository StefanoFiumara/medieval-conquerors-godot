using LiteDB;
using MedievalConquerors.Engine.Data;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Attributes;

public class HitPointsAttribute : CardAttribute
{
    public int Health { get; set; }

    [BsonIgnore] [MapperIgnore]
    public int RemainingHealth { get; set; }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
