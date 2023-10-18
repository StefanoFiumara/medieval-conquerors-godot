using Godot;

namespace MedievalConquerors.Views.Entities;

public partial class CardView : Node2D
{
	private RichTextLabel _title;
	private RichTextLabel _description;
	
	private RichTextLabel _foodCost;
	private RichTextLabel _woodCost;
	private RichTextLabel _goldCost;
	private RichTextLabel _stoneCost;
	
	// TODO: Inject card state object to populate description, title, and cost.
	
	public override void _Ready()
	{
		_title = GetNode<RichTextLabel>("title_banner/title_text");
		_description = GetNode<RichTextLabel>("description_panel/description_text");
		
		_foodCost = GetNode<RichTextLabel>("cost_panel/food_cost");
		_woodCost = GetNode<RichTextLabel>("cost_panel/wood_cost");
		_goldCost = GetNode<RichTextLabel>("cost_panel/gold_cost");
		_stoneCost = GetNode<RichTextLabel>("cost_panel/stone_cost");
	}
}
