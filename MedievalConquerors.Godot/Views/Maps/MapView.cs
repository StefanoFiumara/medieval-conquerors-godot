using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.Utils;
using MedievalConquerors.Views.Main;
using TileData = Godot.TileData;

namespace MedievalConquerors.Views.Maps;

public enum HighlightLayer
{
	MouseHover = 1,
	BlueTeam = 2,
	RedTeam = 3
}

public partial class MapView : TileMap, IGameComponent
{
	private static readonly Vector2I None = new(int.MinValue, int.MinValue);
	
	// TODO: Should we move the highlight tile gfx to a different TileSet?
	//		 Currently it is receiving custom tile data that it is not using.
	private static readonly Vector2I HighlightCoord = new(5, 0);
	private const int TileSetId = 2;

	public IGame Game { get; set; }
	private IMap _map;
	
	private Viewport _viewport;
	
	private Vector2I _hovered = None;
	
	private bool _isDragging = false;
	private Vector2 _dragOffset;
	private Vector2 _zoomTarget;
	private EventAggregator _events;

	public override void _Ready()
	{
		Game = GetParent<GameController>().Game;
		Game.AddComponent(this);
		
		_events = Game.GetComponent<EventAggregator>();
		_map = Game.GetComponent<IMap>();
		_viewport = GetViewport();
		_zoomTarget = Scale;
	}
	
	public override void _Input(InputEvent input)
	{
		if (input.IsEcho()) return;
		if (input is not InputEventMouseButton buttonEvent) return;

		if (HandleMouseInput(buttonEvent))
		{
			_viewport.SetInputAsHandled();
		}
	}

	private bool HandleMouseInput(InputEventMouseButton buttonEvent)
	{
		switch (buttonEvent.ButtonIndex)
		{
			case MouseButton.Left when buttonEvent.IsReleased():
				_events.Publish(InputSystem.ClickedEvent, _map.GetTile(GetTileCoord(buttonEvent.Position)));
				return true;
			case MouseButton.Middle:
				SetDragging(buttonEvent.Pressed);
				return true;
			case MouseButton.WheelUp:
				_zoomTarget *= 1.05f;
				return true;
			case MouseButton.WheelDown:
				_zoomTarget *= 0.95f;
				return true;
			default:
				return false;
		}
	}

	public override void _Process(double elapsed)
	{
		var mousePosition = _viewport.GetMousePosition();
		
		if (!Scale.IsEqualApprox(_zoomTarget))
		{
			var prev = ToLocal(mousePosition);
			var previousScale = Scale;
			Scale = Scale.Lerp(_zoomTarget, 0.2f);
			var cur = ToLocal(mousePosition);
			var diff = cur - prev;
			
			Position += diff * previousScale;
		}
		
		// Drag & zoom map
		if (_isDragging)
		{
			Position = mousePosition + _dragOffset;
		}
		
		// Tile Highlight on hover
		var mapCoord = GetTileCoord(mousePosition);
		if (mapCoord != _hovered)
		{
			RemoveHighlight(_hovered, HighlightLayer.MouseHover);
			_hovered = None;

			if (mapCoord != None)
			{
				HighlightTile(mapCoord, HighlightLayer.MouseHover);
				_hovered = mapCoord;
			}
		}
	}

	private void SetDragging(bool dragging)
	{
		_isDragging = dragging;
		if(_isDragging)
			_dragOffset = Position - _viewport.GetMousePosition();
	}

	private Vector2I GetTileCoord(Vector2 mousePos)
	{
		var mapCoord = LocalToMap(ToLocal(mousePos));
		return _map.GetTile(mapCoord) != null ? mapCoord : None;
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
			// NOTE: The layer ID also matches up with the scene collection ID for the glow color for that layer
			SetCell((int)layer, coord, TileSetId, Vector2I.Zero, (int)layer);
		}
	}

	public bool IsHighlighted(Vector2I coord, HighlightLayer layer)
	{
		if (coord != None)
		{
			// NOTE: Since we are looking at highlight layers, it is ok to simple check if a cell is being used.
			//		 This indicates that it is highlighted, since there is no other reason for a tile to be
			//		 active on this layer.
			return GetUsedCells((int)layer).Contains(coord);
		}

		return false;
	}
	
	public void RemoveHighlight(Vector2I coord, HighlightLayer layer)
	{
		if (coord != None)
		{
			SetCell((int) layer, coord);
		}
	}
	
	public void HighlightTiles(IEnumerable<Vector2I> coords, HighlightLayer layer)
	{
		foreach (var coord in coords)
		{
			HighlightTile(coord, layer);
		}
	}
	
	public void RemoveHighlights(IEnumerable<Vector2I> coords, HighlightLayer layer)
	{
		foreach (var coord in coords)
		{
			RemoveHighlight(coord, layer);
		}
	}
}
