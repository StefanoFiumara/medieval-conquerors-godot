using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.GameData.Cards;

public partial class CardData : Resource, ICardData
{
	[Export] public string Title { get; set; }
	[Export] public string Description { get; set; }
	
	// TODO: Build custom editor window to assign card attribute resources to cards
	[Export] private Resource[] _attributes;
	public IEnumerable<ICardAttribute> Attributes => _attributes.OfType<ICardAttribute>();

	// TODO: Does the engine care about the image? does this need to be in ICardAttribute?
	[Export] public Texture2D Image { get; set; }
}
