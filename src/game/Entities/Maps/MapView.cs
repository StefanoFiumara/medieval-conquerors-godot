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
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Entities.Tokens;
using MedievalConquerors.Extensions;
using MedievalConquerors.Screens;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Entities.Maps;

public partial class MapView : Node2D, IGameComponent
{
	private const int HighlightTileSetId = 1;

	public IGame Game { get; set; }

	private PackedScene _tokenScene;

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

	private Viewport _viewport;
	private Vector2I _hovered = HexMap.None;

	private bool _isDragging;
	private Vector2 _dragOffset;
	private Vector2 _zoomTarget;

	private HexMap _map;
	private ILogger _logger;
	private EventAggregator _events;
	private IGameSettings _settings;

	private readonly List<TokenView> _tokens = [];

	public override void _EnterTree()
	{
		_tokenScene = ResourceLoader.Load<PackedScene>("uid://civascfpgtcfj");

		GetParent<GameController>().Game.AddComponent(this);

		_viewport = GetViewport();
		_logger = Game.GetComponent<ILogger>();
		_map = Game.GetComponent<HexMap>();
		_events = Game.GetComponent<EventAggregator>();
		_settings = Game.GetComponent<IGameSettings>();
	}

	public override void _Ready()
	{
		// TODO: Set scale/position based on reference resolution
		// so that the map fits on the screen at different screen sizes on startup
		_zoomTarget = Scale;

		_map.OnTileChanged += OnTileMapChanged;
		_events.Subscribe<MoveUnitAction>(GameEvent.Prepare<MoveUnitAction>(), OnPrepareMoveUnit);
		_events.Subscribe<GarrisonAction>(GameEvent.Prepare<GarrisonAction>(), OnPrepareGarrison);
		_events.Subscribe<BuildStructureAction>(GameEvent.Prepare<BuildStructureAction>(), OnPrepareBuildStructure);
		_events.Subscribe<SpawnUnitAction>(GameEvent.Prepare<SpawnUnitAction>(), OnPrepareSpawnUnit);
		_events.Subscribe<CollectResourcesAction>(GameEvent.Prepare<CollectResourcesAction>(), OnPrepareCollectResources);
	}

	private void OnPrepareBuildStructure(BuildStructureAction action) => action.PerformPhase.Viewer = BuildStructureAnimation;
	private void OnPrepareSpawnUnit(SpawnUnitAction action) => action.PerformPhase.Viewer = SpawnUnitAnimation;
	private void OnPrepareMoveUnit(MoveUnitAction action) => action.PerformPhase.Viewer = MoveTokenAnimation;
	private void OnPrepareGarrison(GarrisonAction action) => action.PerformPhase.Viewer = GarrisonAnimation;
	private void OnPrepareCollectResources(CollectResourcesAction action) => action.PerformPhase.Viewer = CollectResourcesAnimation;

	public override void _ExitTree()
	{
		_map.OnTileChanged -= OnTileMapChanged;
	}

	private void OnTileMapChanged(TileData oldTile, TileData newTile)
	{
		var newAtlasCoord = _map.GetAtlasCoord(newTile.Terrain);

		if(newAtlasCoord != HexMap.None)
			this[MapLayerType.Terrain].SetCell(newTile.Position, 0, newAtlasCoord);
	}

	public override void _UnhandledInput(InputEvent input)
	{
		if (input.IsEcho()) return;
		if (input is not InputEventMouseButton buttonEvent) return;

		if (HandleMouseInput(buttonEvent))
			_viewport.SetInputAsHandled();
	}

	private bool HandleMouseInput(InputEventMouseButton buttonEvent)
	{
		switch (buttonEvent.ButtonIndex)
		{
			case MouseButton.Left when buttonEvent.IsReleased():
				_events.Publish(InputSystem.ClickedEvent, _map.GetTile(GetTileCoord(buttonEvent.Position)), buttonEvent);
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

				if(_settings.DebugMode)
					CreateTileCoordsPopup(mapCoord);

				_hovered = mapCoord;
			}
		}
	}

	private TokenView CreateTokenView(Card card, Vector2I tile)
	{
		var token = _tokenScene.Instantiate<TokenView>();
		token.Initialize(Game, card);
		token.Position = this[MapLayerType.Terrain].MapToLocal(tile);

		_tokens.Add(token);
		this[MapLayerType.Terrain].AddChild(token);

		return token;
	}

	private Tween PlaceTokenTween(TokenView token, double duration = 0.4)
	{
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
		tween.TweenProperty(token, "modulate", Colors.White, duration).From(Colors.Transparent);
		return tween;
	}

	private IEnumerator BuildStructureAnimation(IGame game, GameAction action)
	{
		var buildAction = (BuildStructureAction) action;
		var token = CreateTokenView(buildAction.StructureToBuild, buildAction.TargetTile);
		var tween = PlaceTokenTween(token);
		while(tween.IsRunning())
			yield return null;
	}

	private IEnumerator SpawnUnitAnimation(IGame game, GameAction action)
	{
		var spawnAction = (SpawnUnitAction) action;
		var token = CreateTokenView(spawnAction.UnitToSpawn, spawnAction.TargetTile);
		var tween = PlaceTokenTween(token);
		while(tween.IsRunning())
			yield return null;
	}

	private IEnumerator MoveTokenAnimation(IGame game, GameAction action)
	{
		const double stepDuration = 0.3;

		var moveAction = (MoveUnitAction) action;
		var path = _map.CalculatePath(moveAction.CardToMove.MapPosition, moveAction.TargetTile);

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

	private IEnumerator GarrisonAnimation(IGame game, GameAction action)
	{
		const double tweenDuration = 0.5;

		var garrisonAction = (GarrisonAction)action;

		var unitToken = CreateTokenView(garrisonAction.Unit, garrisonAction.Building.MapPosition);

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(unitToken, "modulate", Colors.White, tweenDuration).From(Colors.Transparent);
		tween.TweenProperty(unitToken, "modulate", Colors.Transparent, tweenDuration);
		tween.TweenCallback(Callable.From(() =>
		{
			_tokens.Remove(unitToken);
			unitToken.QueueFree();
		}));

		yield return true;

		while (tween.IsRunning())
			yield return null;
	}

	private IEnumerator CollectResourcesAnimation(IGame game, GameAction action)
	{
		const double stepDuration = 0.45;
		var collectAction = (CollectResourcesAction)action;

		yield return true;

		foreach (var collected in collectAction.ResourcesCollected)
		{
			var position = this[MapLayerType.Terrain].MapToLocal(collected.position);
			var tween = this.CreateResourcePopup(position, collected.resource, collected.amount, stepDuration);

			while (tween.IsRunning())
				yield return null;
		}
	}

	private void CreateTileCoordsPopup(Vector2I pos)
	{
		var position = this[MapLayerType.Terrain].MapToLocal(pos);
		this.CreatePopup(position, $"{pos}", duration: 5f, textScale: 0.5f);
	}

	public Vector2 GetTileGlobalPosition(Vector2I coords)
	{
		return this[MapLayerType.Terrain].ToGlobal(this[MapLayerType.Terrain].MapToLocal(coords));
	}

	private Vector2I GetTileCoord(Vector2 mousePos)
	{
		var mapCoord = this[MapLayerType.Terrain].LocalToMap(ToLocal(mousePos));

		if (_map.GetTile(mapCoord) != null)
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
		if(layer == MapLayerType.Terrain)
			_logger.Warn("Attempting to highlight on the terrain layer!");

		// NOTE: The layer ID also matches up with the scene collection ID for the glow color for that layer
		if (coord != HexMap.None)
			this[layer].SetCell(coord, HighlightTileSetId, Vector2I.Zero, (int)layer);
	}

	public bool IsHighlighted(Vector2I coord, MapLayerType layer)
	{
		if(layer == MapLayerType.Terrain)
			_logger.Warn("Attempting to check highlight on the terrain layer!");

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
		if(layer == MapLayerType.Terrain)
			_logger.Warn("Attempting to highlight on the terrain layer!");

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
