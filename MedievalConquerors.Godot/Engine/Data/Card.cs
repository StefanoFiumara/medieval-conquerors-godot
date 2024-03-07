using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Engine.Data;

public interface ICardData
{
    public string Title { get; set; }
    public string Description { get; set; }
    public CardType CardType { get; set; }
    public Tags Tags { get; set; }
    public List<ICardAttribute> Attributes { get; set; }
}

public class CardData : ICardData
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public CardType CardType { get; set; }
    public Tags Tags { get; set; }
    public List<ICardAttribute> Attributes { get; set; } = new();
}

public interface ICardAttribute
{
    // TODO: better name for this?
    void Reset();
    ICardAttribute Clone();
}

public class Card : IGameObject
{
    public ICardData CardData { get; }
    public List<ICardAttribute> Attributes { get; }
    public IPlayer Owner { get; }
    public Zone Zone { get; set; }
    public Vector2I BoardPosition { get; set; }

    public Card(ICardData cardData, IPlayer owner, Zone zone = Zone.None, Vector2I boardPosition = default)
    {
        CardData = cardData;
        Owner = owner;
        Zone = zone;
        BoardPosition = Zone == Zone.Board ? boardPosition : new Vector2I(int.MinValue, int.MinValue);

        Attributes = new List<ICardAttribute>();
        foreach (var dataAttribute in CardData.Attributes)
        {
            Attributes.Add(dataAttribute.Clone());
        }
    }
}