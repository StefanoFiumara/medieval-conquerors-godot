using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Engine.Data;

public interface ICardAttribute
{
    
}

// TODO: Do we need a concrete CardData that isn't a Godot Resource?
public interface ICardData
{
    // TODO: CardType, Tags, Tooltip text
    
    string Title { get; }
    string Description { get; }
    
    IEnumerable<ICardAttribute> Attributes { get; }
}

public class Card : IGameObject
{
    public ICardData CardData { get; }
    
    public IPlayer Owner { get; }
    
    public Zone Zone { get; set; }
    
    public Vector2I BoardPosition { get; set; }
    

    public Card(ICardData cardData, IPlayer owner, Zone zone = Zone.None, Vector2I boardPosition = default)
    {
        CardData = cardData;
        Owner = owner;
        Zone = zone;
        BoardPosition = Zone == Zone.Board ? boardPosition : new Vector2I(int.MinValue, int.MinValue);
    }
}