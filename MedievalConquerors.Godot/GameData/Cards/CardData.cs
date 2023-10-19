using Godot;

namespace MedievalConquerors.GameData.Cards;

public partial class CardData : Resource
{
	[Export] public string Title { get; set; }
	
	[Export] public string Description { get; set; }
	
	// TODO: Build custom editor window to assign card attributes to cards
	[Export] public CardAttribute[] Attributes { get; set; }

	[Export] public Texture2D Image { get; set; }
	
	// TODO: CardType, Tags, Tooltip text
}
