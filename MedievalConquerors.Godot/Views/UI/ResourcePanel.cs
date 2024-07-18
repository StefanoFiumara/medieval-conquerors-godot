using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Views.Main;

namespace MedievalConquerors.Views.UI;

public partial class ResourcePanel : MarginContainer
{
	[Export] private Label _foodLabel;
	[Export] private Label _woodLabel;
	[Export] private Label _goldLabel;
	[Export] private Label _stoneLabel;
	[Export] private PanelContainer _panelContainer;

	private Match _match;
	private Viewport _viewport;
	
	public override void _Ready()
	{
		var game = GetParent<GameController>().Game;
		_match = game.GetComponent<Match>();
	}

	public override void _EnterTree()
	{
		_viewport = GetViewport();
		CalculateViewPosition();
		_viewport.SizeChanged += CalculateViewPosition;
	}

	private void CalculateViewPosition()
	{
		var visibleRect = _viewport.GetVisibleRect();
		_panelContainer.Scale = 0.5f * visibleRect.CalculateScaleFactor();
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
