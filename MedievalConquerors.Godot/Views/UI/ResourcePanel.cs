using System.Text;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;
using MedievalConquerors.Views.Main;

namespace MedievalConquerors.Views.UI;

public partial class ResourcePanel : Control
{
	[Export] private RichTextLabel _label;
	[Export] private PanelContainer _panelContainer;

	private Match _match;
	private Game _game;

	private StringBuilder _sb;
	private Viewport _viewport;
	
	public override void _Ready()
	{
		_game = GetParent<GameController>().Game;
		_match = _game.GetComponent<Match>();
		_sb = new StringBuilder();
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
		_sb.Clear();

		// TODO: apply some string padding to ensure that icons remain aligned when going from single to multiple digits
		_sb.Append("[right]")
			.Append(ResourceIcons.Food).Append(' ').Append(_match.LocalPlayer.Resources.Food)
			.Append(' ')
			.Append(ResourceIcons.Wood).Append(' ').Append(_match.LocalPlayer.Resources.Wood)
			.Append(' ')
			.Append(ResourceIcons.Gold).Append(' ').Append(_match.LocalPlayer.Resources.Gold)
			.Append(' ')
			.Append(ResourceIcons.Stone).Append(' ').Append(_match.LocalPlayer.Resources.Stone)
			.Append("[/right]");
		
		_label.ParseBbcode(_sb.ToString());
	}
}
