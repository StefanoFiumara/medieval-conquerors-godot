using Godot;

namespace MedievalConquerors.Entities.UI;

public partial class PopupLabel : CenterContainer
{
	[Export] public RichTextLabel Label { get; set; }

	public override void _Ready()
	{
		Label.Text = string.Empty;
	}
}
