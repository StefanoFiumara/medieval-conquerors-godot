using System.Linq;
using Godot;
using MedievalConquerors.GameData.Cards;
using MedievalConquerors.GameData.Cards.Attributes;

namespace MedievalConquerors.Views.Entities;

public partial class CardView : Node2D
{
	private RichTextLabel _title;
	private RichTextLabel _description;
	private Sprite2D _image;
	
	private RichTextLabel _foodCost;
	private RichTextLabel _woodCost;
	private RichTextLabel _goldCost;
	private RichTextLabel _stoneCost;

	// TODO: CardData will probably have to be assigned separately, rather than directly via export property
	[Export] private CardData _cardData;
	
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("title_banner/title_text");
		_description = GetNode<RichTextLabel>("description_panel/description_text");
		_image = GetNode<Sprite2D>("image");
		
		_foodCost = GetNode<RichTextLabel>("cost_panel/food_cost");
		_woodCost = GetNode<RichTextLabel>("cost_panel/wood_cost");
		_goldCost = GetNode<RichTextLabel>("cost_panel/gold_cost");
		_stoneCost = GetNode<RichTextLabel>("cost_panel/stone_cost");
		
		_title.Text = _cardData.Title.Center();
		// TODO: Append tags to card description
		_description.Text = _cardData.Description;
		
		// NOTE: Assumes _cardData.Image is a 256x256 sprite.
		_image.Texture = _cardData.Image;

		var cost = _cardData.Attributes.OfType<ResourceCost>().SingleOrDefault();
		if (cost != null)
		{
			_foodCost.Text = $"{cost.Food}".Center();
			_woodCost.Text = $"{cost.Wood}".Center();
			_goldCost.Text = $"{cost.Gold}".Center();
			_stoneCost.Text = $"{cost.Stone}".Center();
		}
	}
}
