namespace MedievalConquerors.Engine.Data.Attributes;

public class HitPointsAttribute : ICardAttribute
{
    public int Health { get; private set; }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }
    
    public void OnTurnStart() { }

    public ICardAttribute Clone()
    {
        return AttributeMapper.Clone(this);
    }
}