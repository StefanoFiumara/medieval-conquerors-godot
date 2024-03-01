using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Extensions;

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
	
	private Card _card;
	
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("container/title_banner/title_text");
		_description = GetNode<RichTextLabel>("container/description_panel/description_text");
		_image = GetNode<Sprite2D>("container/image");
		
		_foodCost = GetNode<RichTextLabel>("container/cost_panel/food_cost");
		_woodCost = GetNode<RichTextLabel>("container/cost_panel/wood_cost");
		_goldCost = GetNode<RichTextLabel>("container/cost_panel/gold_cost");
		_stoneCost = GetNode<RichTextLabel>("container/cost_panel/stone_cost");
	}

	public void Initialize(Card card)
	{
		_card = card;
		_title.Text = _card.CardData.Title.Center();
		// TODO: Append tags to card description
		_description.Text = _card.CardData.Description;
		
		// NOTE: Assumes _cardData.Image is a 256x256 sprite.
		// TODO: Figure out where to load image data from
		//			IDEA: store image resource path in CardData.Image, and load from here? 
		//_image.Texture = _card.CardData.Image;

		var cost = _card.Attributes.OfType<ResourceCostAttribute>().SingleOrDefault();
		if (cost != null)
		{
			_foodCost.Text = $"{cost.Food}".Center();
			_woodCost.Text = $"{cost.Wood}".Center();
			_goldCost.Text = $"{cost.Gold}".Center();
			_stoneCost.Text = $"{cost.Stone}".Center();
		}
	}
}
