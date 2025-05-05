using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

namespace MedievalConquerors.Entities.Editor;

public partial class ImageSelector : HBoxContainer
{
	public const string PortraitsPath = "res://entities/cards/portraits/";
	public const string TokensPath = "res://entities/tokens/token_icons/";

	[Export] private OptionButton _imageOptions;
	[Export] private Button _refreshButton;

	private List<string> _portraitPaths;

	public string SelectedImageUid
	{
		get
		{
			var selected = _imageOptions?.GetItemText(_imageOptions?.GetSelectedId() ?? 0) ?? "None";
			return selected != "None"
				? ResourceUid.IdToText(ResourceLoader.GetResourceUid($"{PortraitsPath}/{selected}"))
				: string.Empty;
		}
		set
		{
			if (string.IsNullOrEmpty(value)) return;

			var id = ResourceUid.TextToId(value);
			if (ResourceUid.HasId(id))
			{
				var fileName = Path.GetFileName(ResourceUid.GetIdPath(id));
				if (_portraitPaths.Contains(fileName))
					_imageOptions?.Select(_portraitPaths.IndexOf(fileName) + 1);
			}
			else
			{
				_imageOptions?.Select(0);
			}
		}
	}

	// TODO: give editor ability to select token manually instead of basing it off of the portrait name
	// This is kinda  nice for now though.
	public string SelectedTokenUid
	{
		get
		{
			var selected = _imageOptions?.GetItemText(_imageOptions?.GetSelectedId() ?? 0) ?? "None";
			return selected != "None"
				? ResourceUid.IdToText(ResourceLoader.GetResourceUid($"{TokensPath}/{selected}"))
				: string.Empty;
		}
	}

	private bool _disabled;
	public bool Disabled
	{
		get => _disabled;
		set
		{
			_disabled = value;
			if(_imageOptions != null) _imageOptions.Disabled = value;
		}
	}

	public override void _Ready()
	{
		_portraitPaths = new();
		RefreshData();
	}

	private void RefreshData()
	{
		_imageOptions.Clear();
		_portraitPaths.Clear();

		_imageOptions.AddItem("None");

		foreach (var image in GetImageList())
		{
			var tex = GD.Load<Texture2D>($"{image.iconUid}");
			_imageOptions.AddIconItem(tex, image.imagePath);

			_portraitPaths.Add(image.imagePath);
		}

		_imageOptions.Select(0);
	}

	private IEnumerable<(string iconUid, string imagePath)> GetImageList()
	{
		var files =  DirAccess.Open(PortraitsPath).GetFiles()
			.GroupBy(p => p.Replace("_icon", string.Empty).Replace(".import", string.Empty))
			.ToDictionary(g => g.Key, g => g.ToList())
			.Where(p => p.Value.Count == 4)
			.Select(p => (imagePath: p.Key, iconPath: p.Key.Replace(".png", "_icon.png")))
			.ToList();

			return files.Select(g => (
				iconUid: ResourceUid.IdToText(ResourceLoader.GetResourceUid($"{PortraitsPath}/{g.iconPath}")),
				g.imagePath
			));
	}

	public override void _EnterTree() => _refreshButton.Pressed += RefreshData;
	public override void _ExitTree() => _refreshButton.Pressed -= RefreshData;
}
