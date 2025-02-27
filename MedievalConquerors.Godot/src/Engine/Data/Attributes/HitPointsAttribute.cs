namespace MedievalConquerors.Engine.Data.Attributes;

public class HitPointsAttribute : ICardAttribute
{
    public int Health { get; set; }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }
    
    public void OnTurnStart() { }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}