using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Views.Entities;

public partial class CardView : Node2D
{
	[Export] private RichTextLabel _title;
	[Export] private RichTextLabel _description;
	[Export] private Sprite2D _image;
	
	[Export] private RichTextLabel _foodCost;
	[Export] private RichTextLabel _woodCost;
	[Export] private RichTextLabel _goldCost;
	[Export] private RichTextLabel _stoneCost;
	
	private Card _card;
	
	public void Initialize(Card card)
	{
		_card = card;
		_title.Text = _card.CardData.Title.Center();
		// TODO: Append tags to card description
		_description.Text = _card.CardData.Description;
		
		// NOTE: Assumes _cardData.Image is a 256x256 sprite.
		if (!string.IsNullOrEmpty(_card.CardData.ImagePath))
		{
			_image.Texture = GD.Load<Texture2D>(_card.CardData.ImagePath);
		}
		
		var cost = _card.Attributes.OfType<ResourceCostAttribute>().SingleOrDefault();
		if (cost != null)
		{
			_foodCost.Text = $"{cost.Food}".Center();
			_woodCost.Text = $"{cost.Wood}".Center();
			_goldCost.Text = $"{cost.Gold}".Center();
			_stoneCost.Text = $"{cost.Stone}".Center();
		}
		else
		{
			// TODO: Hide Resource Panel? Is this ever the case?
		}
	}
}
