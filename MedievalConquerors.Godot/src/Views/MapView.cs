using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.UI;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Views;

public enum MapLayerType
{
	Terrain = 0,
	MouseHover = 1,
	BlueTeam = 2,
	RedTeam = 3,
	SelectionHint = 4
}

public partial class MapView : Node2D, IGameComponent
{
	private const int HighlightTileSetId = 1;

	public IGame Game { get; set; }
	
	[Export] private PackedScene _tokenScene;

	[Export] private TileMapLayer TerrainLayer { get; set; }
	[Export] private TileMapLayer MouseHoverLayer { get; set; }
	[Export] private TileMapLayer BlueTeamLayer { get; set; }
	[Export] private TileMapLayer RedTeamLayer { get; set; }
	[Export] private TileMapLayer SelectionHintLayer { get; set; }

	public TileMapLayer this[MapLayerType layer] => layer switch
	{
		MapLayerType.Terrain => TerrainLayer,
		MapLayerType.MouseHover => MouseHoverLayer,
		MapLayerType.BlueTeam => BlueTeamLayer,
		MapLayerType.RedTeam => RedTeamLayer,
		MapLayerType.SelectionHint => SelectionHintLayer,
		_ => throw new ArgumentOutOfRangeException(nameof(layer), layer, "Invalid map layer type")
	};
	
	// TODO: Make this private
	public HexMap GameMap { get; private set; }
	
	private Viewport _viewport;
	private Vector2I _hovered = HexMap.None;
	
	private bool _isDragging;
	private Vector2 _dragOffset;
	private Vector2 _zoomTarget;
	
	private EventAggregator _events;

	private List<TokenView> _tokens;

	public override void _Ready()
	{
		Game = GetParent<GameController>().Game;
		Game.AddComponent(this);
		
		GameMap = Game.GetComponent<HexMap>();
		
		_events = Game.GetComponent<EventAggregator>();
		_viewport = GetViewport();
		_tokens = new List<TokenView>();
		
		// TODO: Set scale/position based on reference resolution
		// so that the map fits on the screen at different screen sizes on startup
		_zoomTarget = Scale;
		
		GameMap.OnTileChanged += OnTileMapChanged;
		_events.Subscribe<MoveUnitAction>(GameEvent.Prepare<MoveUnitAction>(), OnPrepareMoveUnit);
		_events.Subscribe<GarrisonAction>(GameEvent.Prepare<GarrisonAction>(), OnPrepareGarrison);
		_events.Subscribe<CollectResourcesAction>(GameEvent.Prepare<CollectResourcesAction>(), OnPrepareCollectResources);
	}

	public override void _ExitTree()
	{
		GameMap.OnTileChanged -= OnTileMapChanged;
	}

	private void OnTileMapChanged(TileData oldTile, TileData newTile)
	{
		var newAtlasCoord = GameMap.GetAtlasCoord(newTile.Terrain);
		
		if(newAtlasCoord != HexMap.None)
			this[MapLayerType.Terrain].SetCell(newTile.Position, 0, newAtlasCoord);
	}

	public override void _UnhandledInput(InputEvent input)
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
				_events.Publish(InputSystem.ClickedEvent, GameMap.GetTile(GetTileCoord(buttonEvent.Position)), buttonEvent);
				return true;
			case MouseButton.Middle:
				SetDragging(buttonEvent.Pressed);
				return true;
			case MouseButton.WheelUp:
				_zoomTarget *= 1.05f;
				if (_zoomTarget > Vector2.One) _zoomTarget = Vector2.One;
				return true;
			case MouseButton.WheelDown:
				_zoomTarget *= 0.95f;
				if (_zoomTarget < Vector2.One * 0.6f) _zoomTarget = Vector2.One * 0.6f;
				return true;
			default:
				return false;
		}
	}

	public override void _Process(double elapsed)
	{
		var mousePosition = _viewport.GetMousePosition();
		
		// Zoom
		if (Mathf.Abs(Scale.X - _zoomTarget.X) >= 0.001f)
		{
			var prev = ToLocal(mousePosition);
			var previousScale = Scale;
			Scale = Scale.Lerp(_zoomTarget, (float) elapsed * 15);
			var cur = ToLocal(mousePosition);
			var diff = cur - prev;
			
			Position += diff * previousScale;
		}
		
		// Drag
		if (_isDragging) 
			Position = mousePosition + _dragOffset;
		
		// Tile Highlight on hover
		var mapCoord = GetTileCoord(mousePosition);
		if (mapCoord != _hovered)
		{
			RemoveHighlight(_hovered, MapLayerType.MouseHover);
			_hovered = HexMap.None;

			if (mapCoord != HexMap.None)
			{
				HighlightTile(mapCoord, MapLayerType.MouseHover);
				_hovered = mapCoord;
			}
		}
	}

	private TokenView CreateTokenView(Card card)
	{
		var tokenView = _tokenScene.Instantiate<TokenView>();
		tokenView.Modulate = Colors.Transparent;
		tokenView.Initialize(Game, card);
		
		_tokens.Add(tokenView);
		
		this[MapLayerType.Terrain].AddChild(tokenView);
		return tokenView;
	}

	public Tween PlaceTokenAnimation(PlayCardAction action)
	{
		const double tweenDuration = 0.4;

		var tokenView = CreateTokenView(action.CardToPlay);
		tokenView.Position = this[MapLayerType.Terrain].MapToLocal(action.TargetTile);
		
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
		tween.TweenProperty(tokenView, "modulate", Colors.White, tweenDuration);

		return tween;
	}

	private void OnPrepareMoveUnit(MoveUnitAction action)
	{
		action.PerformPhase.Viewer = MoveTokenAnimation;
	}

	private IEnumerator MoveTokenAnimation(IGame game, GameAction action)
	{
		const double stepDuration = 0.3;
		
		var moveAction = (MoveUnitAction) action;
		var path = GameMap.CalculatePath(moveAction.CardToMove.MapPosition, moveAction.TargetTile);

		var token = _tokens.Single(t => t.Card == moveAction.CardToMove);

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine);

		foreach (var tile in path)
		{
			var stepPosition = this[MapLayerType.Terrain].MapToLocal(tile);
			tween.TweenProperty(token, "position", stepPosition, stepDuration);
		}

		while (tween.IsRunning())
			yield return null;
	}

	private void OnPrepareGarrison(GarrisonAction action)
	{
		action.PerformPhase.Viewer = GarrisonAnimation;
	}

	private IEnumerator GarrisonAnimation(IGame game, GameAction action)
	{
		const double tweenDuration = 0.4;
		
		var garrisonAction = (GarrisonAction)action;
		var unitToken = _tokens.Single(t => t.Card == garrisonAction.Unit);
		var buildingToken = _tokens.Single(t => t.Card == garrisonAction.Building);

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(unitToken, "modulate", Colors.Transparent, tweenDuration);

		yield return true;
		buildingToken.UpdateGarrisonInfo();
		
		while (tween.IsRunning())
			yield return null;
		
		_tokens.Remove(unitToken);
		unitToken.QueueFree();
	}

	private void OnPrepareCollectResources(CollectResourcesAction action)
	{
		action.PerformPhase.Viewer = CollectResourcesAnimation;
	}

	private IEnumerator CollectResourcesAnimation(IGame game, GameAction action)
	{
		const double stepDuration = 0.45;
		var collectAction = (CollectResourcesAction)action;

		yield return true;
		
		foreach (var collected in collectAction.ResourcesCollected)
		{
			var position = this[MapLayerType.Terrain].MapToLocal(collected.Key);
			var (resource, amount) = collected.Value;
			var tween = this.CreateResourcePopup(position, resource, amount, stepDuration);

			while (tween.IsRunning())
				yield return null;
		}
	}


	public Vector2 GetTileGlobalPosition(Vector2I coords)
	{
		return this[MapLayerType.Terrain].ToGlobal(this[MapLayerType.Terrain].MapToLocal(coords));
	}

	private Vector2I GetTileCoord(Vector2 mousePos)
	{
		var mapCoord = this[MapLayerType.Terrain].LocalToMap(ToLocal(mousePos));
		
		if (GameMap.GetTile(mapCoord) != null)
			return mapCoord;
		
		return HexMap.None;
	}

	private void SetDragging(bool dragging)
	{
		_isDragging = dragging;
		if(_isDragging)
			_dragOffset = Position - _viewport.GetMousePosition();
	}

	public void Clear(MapLayerType layer)
	{
		var cells = this[layer].GetUsedCells();

		foreach (var cell in cells) 
			RemoveHighlight(cell, layer);
	}

	public void HighlightTile(Vector2I coord, MapLayerType layer)
	{
		// NOTE: The layer ID also matches up with the scene collection ID for the glow color for that layer
		if (coord != HexMap.None) 
			this[layer].SetCell(coord, HighlightTileSetId, Vector2I.Zero, (int)layer);
	}

	public bool IsHighlighted(Vector2I coord, MapLayerType layer)
	{
		if (coord != HexMap.None)
		{
			// NOTE: Since we are looking at highlight layers, it is ok to simple check if a cell is being used.
			//		 This indicates that it is highlighted, since there is no other reason for a tile to be
			//		 active on this layer.
			return this[layer].GetUsedCells().Contains(coord);
		}

		return false;
	}
	
	public void RemoveHighlight(Vector2I coord, MapLayerType layer)
	{
		if (coord != HexMap.None) 
			this[layer].SetCell(coord);
	}
	
	public void HighlightTiles(IEnumerable<Vector2I> coords, MapLayerType layer)
	{
		foreach (var coord in coords) 
			HighlightTile(coord, layer);
	}
	
	public void RemoveHighlights(IEnumerable<Vector2I> coords, MapLayerType layer)
	{
		foreach (var coord in coords) 
			RemoveHighlight(coord, layer);
	}
}
