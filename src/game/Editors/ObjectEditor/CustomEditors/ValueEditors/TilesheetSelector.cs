using Godot;

namespace MedievalConquerors.Editors.CustomEditors.ValueEditors;

/// <summary>
/// Allows selecting a tile from a spritesheet/tilesheet.
/// The returned index corresponds to Sprite2D.Frame ordering:
/// 0-based, left-to-right, top-to-bottom (index = row * Columns + col).
/// </summary>
public partial class TilesheetSelector : OptionButton, IValueEditor
{
	private const int IconMaxWidth = 32;
	private const string MissingIcon = "uid://dnofkwp8xw7ys";

	[Export] public Texture2D Tilesheet { get; set; }

	[Export] public int Columns { get; set; } = 1;
	[Export] public int Rows { get; set; } = 1;

	[Export] public int NumTilesToShow { get; set; } = -1;

	public override void _Ready()
	{
		ExpandIcon = true;
		LoadTiles();
	}

	private void LoadTiles()
	{
		Clear();

		if (Tilesheet == null)
		{
			GD.PrintErr("TilesheetSelector: No tilesheet texture assigned.");
			Select(0);
			return;
		}

		if (Columns <= 0 || Rows <= 0)
		{
			GD.PrintErr("TilesheetSelector: Columns and Rows must be > 0.");
			Select(0);
			return;
		}

		var texSize = Tilesheet.GetSize();
		var cellW = texSize.X / Columns;
		var cellH = texSize.Y / Rows;
		var popup = GetPopup();

		AddIconItem(GD.Load<Texture2D>(MissingIcon), "None");
		popup.SetItemIconMaxWidth(0, IconMaxWidth);

		var tilesAdded = 0;

		for (int row = 0; row < Rows; row++)
		{
			for (int col = 0; col < Columns; col++)
			{
				int frameIndex = row * Columns + col;

				var atlas = new AtlasTexture
				{
					Atlas = Tilesheet,
					Region = new Rect2(col * cellW, row * cellH, cellW, cellH)
				};

				AddIconItem(atlas, $"Tile {frameIndex}", frameIndex);

				// Item index in the OptionButton = frameIndex + 1 (due to "None" at 0)
				popup.SetItemIconMaxWidth(frameIndex + 1, IconMaxWidth);

				tilesAdded++;
				if (NumTilesToShow > -1 && tilesAdded >= NumTilesToShow)
					goto end;
			}
		}

		end:
		Select(0);
	}

	public Control GetControl() => this;

	public object GetValue()
	{
		var id = GetSelectedId();
		return id; // -1 for "None", otherwise the frame index
	}

	public void SetValue(object value)
	{
		if (value is int frameIndex && frameIndex >= 0)
		{
			var itemIndex = GetItemIndex(frameIndex);
			if (itemIndex >= 0)
				Select(itemIndex);
			else
				Select(0);
		}
		else
		{
			Select(0);
		}
	}

	public void Enable() => Disabled = false;
	public void Disable() => Disabled = true;
}
