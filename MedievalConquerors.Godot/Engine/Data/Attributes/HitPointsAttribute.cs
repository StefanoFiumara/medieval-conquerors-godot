namespace MedievalConquerors.Engine.Data.Attributes;

// TODO: System to use this attribute
public class HitPointsAttribute : ICardAttribute
{
    public int Health { get; private set; }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }
    
    public void Reset()
    {
        
    }

    public ICardAttribute Clone()
    {
        return AttributeMapper.Clone(this);
    }
}