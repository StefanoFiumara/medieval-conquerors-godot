using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Views;

namespace MedievalConquerors;

public partial class GameController : Node
{
	[Export] private LogLevel _logLevel;
	[Export] private GameSettings _settings;
	
	// IDEA: We can load different maps through this resource
	[Export] private MapView _mapView;
	
	private Game _game;
	private ILogger _log;
	private HexMap _map;

	public Game Game => _game;

	public override void _EnterTree()
	{ 
		_log = new GodotLogger(_logLevel);
		_map = GameMapFactory.CreateHexMap(_mapView[MapLayerType.Terrain]);
		_game = GameFactory.Create(_log, _map, _settings);
	}

	public override void _Ready()
	{
		_game.Awake();
		_game.Perform(new BeginGameAction(Match.LocalPlayerId));
	}

	public override void _Process(double elapsed)
	{
		_game.Update();
	}
	
	public override void _ExitTree()
	{
		_game.Destroy();
	}
}
