using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.GameComponents;

namespace MedievalConquerors.Engine.Data;

public interface ICardData
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public CardType CardType { get; set; }
    public Tags Tags { get; set; }
    public List<ICardAttribute> Attributes { get; set; }
}

public class CardData : ICardData
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public CardType CardType { get; set; }
    public Tags Tags { get; set; }
    public List<ICardAttribute> Attributes { get; set; } = new();
}

public interface ICardAttribute
{
    void Reset();
    ICardAttribute Clone();
}

public class Card
{
    public ICardData CardData { get; }
    public List<ICardAttribute> Attributes { get; }
    public IPlayer Owner { get; }
    public Zone Zone { get; set; }
    public Vector2I MapPosition { get; set; }

    public Card(ICardData cardData, IPlayer owner, Zone zone = Zone.None, Vector2I mapPosition = default)
    {
        CardData = cardData;
        Owner = owner;
        Zone = zone;
        MapPosition = Zone == Zone.Map ? mapPosition : MapSystem.InvalidTile;

        Attributes = new List<ICardAttribute>();
        foreach (var dataAttribute in CardData.Attributes)
        {
            Attributes.Add(dataAttribute.Clone());
        }
    }
}