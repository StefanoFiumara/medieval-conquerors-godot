using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

namespace MedievalConquerors.Editors.CustomEditors.ValueEditors;

public partial class ImageSelector : OptionButton, IValueEditor
{
	private const int IconMaxWidth = 32;
	private const string MissingIcon = "uid://dnofkwp8xw7ys";

	public static readonly string None = string.Empty;

	private readonly List<string> _filePaths = [];

	[Export] public string SourcePath { get; set; }

	public override void _Ready()
	{
		ExpandIcon = true;
		LoadImages();
	}

	private void LoadImages()
	{
		Clear();
		_filePaths.Clear();

		var dir = DirAccess.Open(SourcePath);
		if (dir == null)
		{
			GD.PrintErr($"ImageSelector: Could not open directory '{SourcePath}'");
			Select(0);
			return;
		}

		var popup = GetPopup();

		AddIconItem(GD.Load<Texture2D>(MissingIcon), "None");
		popup.SetItemIconMaxWidth(0, IconMaxWidth);

		var files = dir.GetFiles()
			.Where(f => !f.EndsWith(".import") && !f.EndsWith(".uid"))
			.OrderBy(f => f)
			.ToList();

		for (int i = 0; i < files.Count; i++)
		{
			var fileName = files[i];
			var fullPath = $"{SourcePath}/{fileName}";
			var tex = GD.Load<Texture2D>(fullPath);

			if (tex != null)
			{
				AddIconItem(tex, fileName);
				popup.SetItemIconMaxWidth(i + 1, IconMaxWidth); // +1 for "None" entry
				_filePaths.Add(fileName);
			}
		}

		Select(0);
	}

	public Control GetControl() => this;

	public object GetValue()
	{
		var selected = GetItemText(GetSelectedId());
		if (selected == "None")
			return None;

		var fullPath = $"{SourcePath}/{selected}";
		var uid = ResourceLoader.GetResourceUid(fullPath);
		return uid != ResourceUid.InvalidId
			? ResourceUid.IdToText(uid)
			: None;
	}

	public void SetValue(object value)
	{
		if (value is not string uidText || string.IsNullOrEmpty(uidText))
		{
			Select(0);
			return;
		}

		var id = ResourceUid.TextToId(uidText);
		if (id == ResourceUid.InvalidId || !ResourceUid.HasId(id))
		{
			Select(0);
			return;
		}

		var fileName = Path.GetFileName(ResourceUid.GetIdPath(id));
		var idx = _filePaths.IndexOf(fileName);
		Select(idx >= 0 ? idx + 1 : 0); // +1 for "None" entry
	}

	public void Enable() => Disabled = false;
	public void Disable() => Disabled = true;
}
