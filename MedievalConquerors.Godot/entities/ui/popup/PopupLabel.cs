using Godot;

namespace MedievalConquerors.Entities.UI.popup;

public partial class PopupLabel : CenterContainer
{
	[Export] public RichTextLabel Label { get; set; }

	public override void _Ready()
	{
		Label.Text = string.Empty;
	}
}
