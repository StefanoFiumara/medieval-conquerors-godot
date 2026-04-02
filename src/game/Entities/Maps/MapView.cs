using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Entities.Tokens;
using MedievalConquerors.Screens;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Entities.Maps;

// TODO: Split animations into helper classes, similar to HandView
public partial class MapView : Node2D, IGameComponent
{
	private static readonly PackedScene _tokenScene = GD.Load<PackedScene>("uid://civascfpgtcfj");

	private const int HIGHLIGHT_TILE_SET_ID = 1;
	private const int HIGHLIGHT_TILE = 1;
	private readonly Vector2 _minZoom = Vector2.One * 0.5f;
	private readonly Vector2 _maxZoom = Vector2.One * 1.0f;

	public IGame Game { get; set; }

	// TODO: Hook this up via GetNode with unique names
	[Export] private TileMapLayer TerrainLayer { get; set; }
	[Export] private TileMapLayer MouseHoverLayer { get; set; }
	[Export] private TileMapLayer SelectionHintLayer { get; set; }

	public TileMapLayer this[MapLayerType layer] => layer switch
	{
		MapLayerType.Terrain => TerrainLayer,
		MapLayerType.MouseHover => MouseHoverLayer,
		MapLayerType.SelectionHint => SelectionHintLayer,
		_ => throw new ArgumentOutOfRangeException(nameof(layer), layer, "Invalid map layer type")
	};

	private Viewport _viewport;
	public Vector2I HoveredTile { get; private set; } = HexMap.None;

	private bool _isDragging;
	private Vector2 _dragOffset;
	private Vector2 _zoomTarget;

	private HexMap _map;
	private Match _match;
	private ILogger _logger;
	private EventAggregator _events;
	private IGameSettings _settings;

	private readonly List<TokenView> _tokens = [];

	public override void _EnterTree()
	{
		GetParent<GameController>().Game.AddComponent(this);

		_viewport = GetViewport();
		_logger = Game.GetComponent<ILogger>();
		_map = Game.GetComponent<HexMap>();
		_match = Game.GetComponent<Match>();
		_events = Game.GetComponent<EventAggregator>();
		_settings = Game.GetComponent<IGameSettings>();
	}

	public override void _Ready()
	{
		// TODO: Set scale/position based on reference resolution
		// so that the map fits on the screen at different screen sizes on startup
		_zoomTarget = Scale;

		_map.OnTileChanged += OnTileMapChanged;
		_events.Subscribe<ResetSpentVillagersAction>(GameEvent.Prepare<ResetSpentVillagersAction>(), OnPrepareResetSpentVillagers);
		_events.Subscribe<HarvestAction>(GameEvent.Prepare<HarvestAction>(), OnPrepareCollectResource);
		_events.Subscribe<MoveUnitAction>(GameEvent.Prepare<MoveUnitAction>(), OnPrepareMoveUnit);
		_events.Subscribe<GarrisonAction>(GameEvent.Prepare<GarrisonAction>(), OnPrepareGarrison);
		_events.Subscribe<BuildStructureAction>(GameEvent.Prepare<BuildStructureAction>(), OnPrepareBuildStructure);
		_events.Subscribe<SpawnUnitAction>(GameEvent.Prepare<SpawnUnitAction>(), OnPrepareSpawnUnit);
	}

	private void OnPrepareResetSpentVillagers(ResetSpentVillagersAction action) => action.PerformPhase.Viewer = ResetSpentTokensAnimation;
	private void OnPrepareCollectResource(HarvestAction action) => action.PerformPhase.Viewer = SpendVillagerTokenAnimation;
	private void OnPrepareBuildStructure(BuildStructureAction action) => action.PerformPhase.Viewer = BuildStructureAnimation;
	private void OnPrepareSpawnUnit(SpawnUnitAction action) => action.PerformPhase.Viewer = SpawnUnitAnimation;
	private void OnPrepareMoveUnit(MoveUnitAction action) => action.PerformPhase.Viewer = MoveTokenAnimation;
	private void OnPrepareGarrison(GarrisonAction action) => action.PerformPhase.Viewer = GarrisonAnimation;

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

	// TODO: Refactor to match what is being done in HandView
	//		i.e. Fire movement action when selected unit is dragged to new tile
	public override void _UnhandledInput(InputEvent input)
	{
		if (input.IsEcho()) return;
		if (input is not InputEventMouseButton buttonEvent) return;

		// TODO: Use input actions here
		if (HandleMouseInput(buttonEvent))
			_viewport.SetInputAsHandled();
	}

	private bool HandleMouseInput(InputEventMouseButton buttonEvent)
	{
		switch (buttonEvent.ButtonIndex)
		{
			case MouseButton.Left when buttonEvent.IsReleased():
				// _events.Publish(InputSystem.CLICKED_EVENT, _map.GetTile(GetTileCoord(buttonEvent.Position)), buttonEvent);
				return true;
			case MouseButton.Middle:
				SetDragging(buttonEvent.Pressed);
				return true;
			case MouseButton.WheelUp:
				_zoomTarget *= 1.05f;
				if (_zoomTarget > _maxZoom) _zoomTarget = _maxZoom;
				return true;
			case MouseButton.WheelDown:
				_zoomTarget *= 0.95f;
				if (_zoomTarget < _minZoom) _zoomTarget = _minZoom;
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
		if (mapCoord != HoveredTile)
		{
			RemoveHighlight(HoveredTile, MapLayerType.MouseHover);
			HoveredTile = HexMap.None;

			if (mapCoord != HexMap.None)
			{
				HighlightTile(mapCoord, MapLayerType.MouseHover);

				if(_settings.DebugShowTileCoords)
					CreateTileCoordsPopup(mapCoord);

				HoveredTile = mapCoord;
			}
		}
	}

	private TokenView CreateTokenView(Card card, Vector2I tile)
	{
		var token = _tokenScene.Instantiate<TokenView>();
		token.Position = this[MapLayerType.Terrain].MapToLocal(tile);

		_tokens.Add(token);
		this[MapLayerType.Terrain].AddChild(token);

		token.Initialize(Game, card);
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
		const double STEP_DURATION = 0.3;

		var moveAction = (MoveUnitAction) action;
		var path = _map.CalculatePath(moveAction.CardToMove.MapPosition, moveAction.TargetTile);

		var token = _tokens.Single(t => t.Card == moveAction.CardToMove);

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine);
		foreach (var tile in path)
		{
			var stepPosition = this[MapLayerType.Terrain].MapToLocal(tile);
			tween.TweenProperty(token, "position", stepPosition, STEP_DURATION);
		}

		while (tween.IsRunning())
			yield return null;
	}

	private IEnumerator GarrisonAnimation(IGame game, GameAction action)
	{
		var garrisonAction = (GarrisonAction)action;
		yield return true;

		var buildingToken = _tokens.Single(t => t.Card == garrisonAction.Building);
		var garrisonSystem = Game.GetComponent<GarrisonSystem>();

		var unitCount = garrisonSystem.GetGarrisonedUnits(garrisonAction.Building).Count;
		buildingToken.SetGarrisonView(unitCount);
	}

	private IEnumerator ResetSpentTokensAnimation(IGame game, GameAction action)
	{
		var resetAction = (ResetSpentVillagersAction) action;
		foreach (var token in _tokens.Where(t => t.Card.Owner.Id == resetAction.PlayerId))
		{
			token.ResetSpentGarrison();
		}
		yield return true;
	}

	private IEnumerator SpendVillagerTokenAnimation(IGame game, GameAction action)
	{
		var spendAction = (HarvestAction)action;

		var token = _tokens.Single(t => t.Card.MapPosition == spendAction.TargetTile);
		token.MarkGarrisonedAsSpent();
		yield return true;
	}

	private void CreateTileCoordsPopup(Vector2I pos)
	{
		var position = this[MapLayerType.Terrain].MapToLocal(pos);
		this.CreatePopup(position, $"{pos}", duration: 1.5f, textScale: 0.6f);
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

		if (coord != HexMap.None)
			this[layer].SetCell(coord, HIGHLIGHT_TILE_SET_ID, Vector2I.Zero, HIGHLIGHT_TILE);
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

	public void ResetSelection(MapLayerType layer)
	{
		foreach (var coord in this[layer].GetUsedCells())
		{
			RemoveHighlight(coord, layer);
		}
	}
}
