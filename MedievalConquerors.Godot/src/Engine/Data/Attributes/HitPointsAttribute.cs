namespace MedievalConquerors.Engine.Data.Attributes;

public class HitPointsAttribute : CardAttribute
{
    public int Health { get; set; }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }

    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
