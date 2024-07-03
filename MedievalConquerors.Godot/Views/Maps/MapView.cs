using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.Views.Entities;
using MedievalConquerors.Views.Main;

namespace MedievalConquerors.Views.Maps;

public enum HighlightLayer
{
	MouseHover = 1,
	BlueTeam = 2,
	RedTeam = 3,
	TileSelectionHint = 4
}

public partial class MapView : Node2D, IGameComponent
{
	private const int HighlightTileSetId = 2;

	public IGame Game { get; set; }
	
	[Export] private PackedScene _tokenScene;
	[Export] public TileMap TileMap { get; private set; }
	
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
		_zoomTarget = Scale;

		_tokens = new();
		
		_events.Subscribe<MoveUnitAction>(GameEvent.Prepare<MoveUnitAction>(), OnPrepareMoveUnit);
		_events.Subscribe<GarrisonAction>(GameEvent.Prepare<GarrisonAction>(), OnPrepareGarrison);
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
				_events.Publish(InputSystem.ClickedEvent, GameMap.GetTile(GetTileCoord(buttonEvent.Position)), buttonEvent);
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
			_hovered = HexMap.None;

			if (mapCoord != HexMap.None)
			{
				HighlightTile(mapCoord, HighlightLayer.MouseHover);
				_hovered = mapCoord;
			}
		}
	}

	private TokenView CreateTokenView(Card card)
	{
		var tokenView = _tokenScene.Instantiate<TokenView>();
		
		// Spawn token offscreen, to be animated in by TweenToPosition
		// TODO: Adjust offscreen position for animation
		tokenView.Position = (Vector2.Up * 1200);
		
		TileMap.AddChild(tokenView);
		tokenView.Initialize(Game, card);
		
		_tokens.Add(tokenView);
		return tokenView;
	}

	public Tween PlaceTokenAnimation(PlayCardAction action)
	{
		const double tweenDuration = 0.4;

		var tokenView = CreateTokenView(action.CardToPlay);
		tokenView.Position = TileMap.MapToLocal(action.TargetTile);
		tokenView.Modulate = Colors.Transparent;
		
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
			var stepPosition = TileMap.MapToLocal(tile);
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
		var garrisonAction = (GarrisonAction)action;
		var unitToken = _tokens.Single(t => t.Card == garrisonAction.Unit);

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(unitToken, "modulate", Colors.Transparent, 0.2);

		while (tween.IsRunning())
			yield return null;

		// TODO: Update building token's UI to show garrisoned unit count
		_tokens.Remove(unitToken);
		unitToken.QueueFree();
	}

	public Vector2 GetTileGlobalPosition(Vector2I coords)
	{
		return TileMap.ToGlobal(TileMap.MapToLocal(coords));
	}

	private Vector2I GetTileCoord(Vector2 mousePos)
	{
		var mapCoord = TileMap.LocalToMap(ToLocal(mousePos));
		return GameMap.GetTile(mapCoord) != null ? mapCoord : HexMap.None;
	}

	private void SetDragging(bool dragging)
	{
		_isDragging = dragging;
		if(_isDragging)
			_dragOffset = Position - _viewport.GetMousePosition();
	}

	public void Clear(HighlightLayer layer)
	{
		var cells = TileMap.GetUsedCells((int)layer);

		foreach (var cell in cells)
		{
			RemoveHighlight(cell, layer);
		}
	}

	public void HighlightTile(Vector2I coord, HighlightLayer layer)
	{
		if (coord != HexMap.None)
		{
			// NOTE: The layer ID also matches up with the scene collection ID for the glow color for that layer
			TileMap.SetCell((int)layer, coord, HighlightTileSetId, Vector2I.Zero, (int)layer);
		}
	}

	public bool IsHighlighted(Vector2I coord, HighlightLayer layer)
	{
		if (coord != HexMap.None)
		{
			// NOTE: Since we are looking at highlight layers, it is ok to simple check if a cell is being used.
			//		 This indicates that it is highlighted, since there is no other reason for a tile to be
			//		 active on this layer.
			return TileMap.GetUsedCells((int)layer).Contains(coord);
		}

		return false;
	}
	
	public void RemoveHighlight(Vector2I coord, HighlightLayer layer)
	{
		if (coord != HexMap.None)
		{
			TileMap.SetCell((int) layer, coord);
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
