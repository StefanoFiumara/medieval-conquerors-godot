using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public class HitPointsAttribute : CardAttribute
{
    public int Health { get; set; }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
