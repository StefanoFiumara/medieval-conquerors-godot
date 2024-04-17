using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

namespace MedievalConquerors.ConquerorsPlugin.Controls;

[Tool]
public partial class ImageSelector : HBoxContainer
{
	private const string RootPath = "res://Assets/CardImages";

	[Export] private OptionButton _imageOptions;
	[Export] private Button _refreshButton;

	private List<string> _imagePaths;
	
	public string SelectedImagePath
	{
		get
		{
			var selected = _imageOptions?.GetItemText(_imageOptions?.GetSelectedId() ?? 0) ?? "None";
			return selected != "None" ? $"{RootPath}/{selected}" : string.Empty;
		}
		set
		{
			if (string.IsNullOrEmpty(value)) return;
			
			var fileName = Path.GetFileName(value);
			if (_imagePaths.Contains(fileName))
			{
				_imageOptions?.Select(_imagePaths.IndexOf(fileName) + 1);
			}
			else
			{
				_imageOptions?.Select(0);
			}
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
		_imagePaths = new();
		RefreshData();
	}

	public override void _EnterTree() => _refreshButton.Pressed += RefreshData;
	public override void _ExitTree() => _refreshButton.Pressed -= RefreshData;

	private void RefreshData()
	{
		_imageOptions.Clear();
		_imagePaths.Clear();

		_imageOptions.AddItem("None");

		foreach (var image in GetImageList())
		{
			var tex = GD.Load<Texture2D>($"res://Assets/CardImages/{image.iconPath}");
			_imageOptions.AddIconItem(tex, image.imagePath);
			_imagePaths.Add(image.imagePath);
		}
		
		_imageOptions.Select(0);
	}

	private IEnumerable<(string imagePath, string iconPath)> GetImageList()
	{
		return DirAccess.Open(RootPath).GetFiles()
			.GroupBy(p => p.Replace("_icon", string.Empty).Replace(".import", string.Empty))
			.ToDictionary(g => g.Key, g => g.ToList())
			.Where(p => p.Value.Count == 4)
			.Select(p => (imagePath: p.Key, iconPath: p.Key.Replace(".png", "_icon.png")));	
	}
}
