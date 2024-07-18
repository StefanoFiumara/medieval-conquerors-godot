using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Views.Main;

namespace MedievalConquerors.Views.UI;

public partial class ResourcePanel : MarginContainer
{
	[Export] private GameController _gameController;
	
	private Label _foodLabel;
	private Label _woodLabel;
	private Label _goldLabel;
	private Label _stoneLabel;

	private Match _match;
	private Viewport _viewport;
	
	public override void _Ready()
	{
		_foodLabel = GetNode<Label>("%food_label");
		_woodLabel = GetNode<Label>("%wood_label");
		_goldLabel = GetNode<Label>("%gold_label");
		_stoneLabel = GetNode<Label>("%stone_label");
		
		_match = _gameController.Game.GetComponent<Match>();
	}

	public override void _Process(double delta)
	{
		// TODO: Display storage limit (find icon)
		_foodLabel.Text = _match.LocalPlayer.Resources.Food.ToString();
		_woodLabel.Text = _match.LocalPlayer.Resources.Wood.ToString();
		_goldLabel.Text = _match.LocalPlayer.Resources.Gold.ToString();
		_stoneLabel.Text = _match.LocalPlayer.Resources.Stone.ToString();
	}
}
