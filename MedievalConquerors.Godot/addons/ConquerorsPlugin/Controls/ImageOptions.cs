using System.IO;
using System.Linq;
using Godot;

namespace MedievalConquerors.ConquerorsPlugin.Controls;

[Tool]
public partial class ImageOptions : OptionButton
{
	private const string RootPath = "res://Assets/CardImages";
	public override void _Ready()
	{
		AddItem("None");

		var cardImages = DirAccess.Open(RootPath)
			.GetFiles()
			.Where(p => p.EndsWith("_icon.png"));
		
		foreach (var path in cardImages)
		{
			var tex = GD.Load<Texture2D>($"res://Assets/CardImages/{path}");
			AddIconItem(tex, Path.GetFileName(path));
		}
	}
}