using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;

using GameSettings = MedievalConquerors.Engine.Data.GameSettings;
using MapView = MedievalConquerors.entities.maps.MapView;

namespace MedievalConquerors;

public partial class GameController : Node
{
	[Export] private LogLevel _logLevel;
	[Export] private GameSettings _settings;

	// IDEA: We can load different maps through this resource
	[Export] private MapView _mapView;

	private ILogger _log;
	private HexMap _map;

	public Game Game { get; private set; }

	public override void _EnterTree()
	{
		_log = new GodotLogger(_logLevel);
		_map = GameMapFactory.CreateHexMap(_mapView[MapLayerType.Terrain]);
		Game = GameFactory.Create(_log, _map, _settings);
	}

	public override void _Ready()
	{
		Game.Awake();
		Game.Perform(new BeginGameAction(Match.LocalPlayerId));
	}

	public override void _Process(double elapsed) => Game.Update();
	public override void _ExitTree() => Game.Destroy();
}
