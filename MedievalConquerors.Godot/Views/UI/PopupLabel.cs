using Godot;

namespace MedievalConquerors.Views.UI;

public partial class PopupLabel : Node2D
{
	[Export] public RichTextLabel Label { get; set; }

	public override void _Ready()
	{
		Label.Text = string.Empty;
	}
}
