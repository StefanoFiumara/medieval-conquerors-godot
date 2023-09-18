using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Views.Maps;

public enum HighlightLayer
{
	MouseHover = 1,
	RangeVisualizer = 2,
	BlueTeam = 3,
	RedTeam = 4
}

public partial class MapView : TileMap
{
	private static readonly Vector2I None = new(int.MinValue, int.MinValue);
	
	// TODO: Should we move the highlight tile gfx to a different TileSet?
	//		 Currently it is receiving custom tile data that it is not using.
	private static readonly Vector2I AtlasCoord = new(5, 0);
	private const int TileSetId = 1;
	
	private Viewport _viewport;
	private Vector2I _hovered = None;
	
	public override void _Ready()
	{
		_viewport = GetViewport();
	}
	
	public override void _Process(double delta)
	{
		var mousePos = ToLocal(_viewport.GetMousePosition());
		var coord = LocalToMap(mousePos);

		if (coord != _hovered)
		{
			RemoveHighlight(_hovered, HighlightLayer.MouseHover);
			HighlightTile(coord, HighlightLayer.MouseHover);
			_hovered = coord;
		}
	}

	public void Visualize(IEnumerable<Vector2I> coords)
	{
		foreach (var coord in coords)
		{
			HighlightTile(coord, HighlightLayer.RangeVisualizer);
		}
	}

	public void Clear(HighlightLayer layer)
	{
		var cells = GetUsedCells((int)layer);

		foreach (var cell in cells)
		{
			RemoveHighlight(cell, layer);
		}
	}

	public void HighlightTile(Vector2I coord, HighlightLayer layer)
	{
		if (coord != None)
		{
			SetCell((int)layer, coord, TileSetId, AtlasCoord);
		}
	}

	public void RemoveHighlight(Vector2I coord, HighlightLayer layer)
	{
		if (coord != None)
		{
			SetCell((int) layer, coord);
		}
	}
}
