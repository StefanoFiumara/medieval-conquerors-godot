using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.GameData.Cards;

public class CardData : Resource
{
    [Export] public string Title { get; set; }
    
    [Export] public string Description { get; set; }
    
    [Export] public List<CardAttribute> Attributes { get; set; }

    [Export] public Sprite2D Image { get; set; }
    
    // TODO: CardType, Tags, Tooltip text
}