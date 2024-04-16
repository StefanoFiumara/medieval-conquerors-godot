using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Views.Entities;

public partial class CardView : Node2D
{
	[Export] private Label _title;
	[Export] private RichTextLabel _description;
	[Export] private Sprite2D _image;
	
	[Export] private Label _foodCost;
	[Export] private Label _woodCost;
	[Export] private Label _goldCost;
	[Export] private Label _stoneCost;
	
	[Export] private TextureRect _foodIcon;
	[Export] private TextureRect _woodIcon;
	[Export] private TextureRect _goldIcon;
	[Export] private TextureRect _stoneIcon;
	
	private Card _card;
	
	public void Initialize(Card card)
	{
		_card = card;
		
		// TODO: Update title color based on card type?
		_title.Text = _card.CardData.Title;
		// TODO: Append tags and card type to description
		_description.Text = _card.CardData.Description;
		
		// NOTE: Assumes _cardData.Image is a 256x256 sprite.
		if (!string.IsNullOrEmpty(_card.CardData.ImagePath))
		{
			_image.Texture = GD.Load<Texture2D>(_card.CardData.ImagePath);
		}
		
		var cost = _card.Attributes.OfType<ResourceCostAttribute>().SingleOrDefault();
		if (cost != null)
		{
			_foodCost.Text = $"{cost.Food}";
			_woodCost.Text = $"{cost.Wood}";
			_goldCost.Text = $"{cost.Gold}";
			_stoneCost.Text = $"{cost.Stone}";
			
			_foodIcon.Visible = cost.Food > 0;
			_woodIcon.Visible = cost.Wood > 0;
			_goldIcon.Visible = cost.Gold > 0;
			_stoneIcon.Visible = cost.Stone > 0;
			
			_foodCost.Visible = cost.Food > 0;
			_woodCost.Visible = cost.Wood > 0;
			_goldCost.Visible = cost.Gold > 0;
			_stoneCost.Visible = cost.Stone > 0;
		}
		else
		{
			// TODO: Hide Resource Panel? Is this ever the case?
		}
	}
}
