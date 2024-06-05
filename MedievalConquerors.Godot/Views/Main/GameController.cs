using System.Linq;
using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Views.Maps;

namespace MedievalConquerors.Views.Main;

public partial class GameController : Node
{
	[Export] private LogLevel _logLevel;
	[Export] private GameSettings _settings;
	
	// IDEA: We can load different maps through this resource
	[Export] private MapView _mapView;
	
	private Game _game;
	private ILogger _log;
	private IGameMap _map;

	public Game Game => _game;

	public override void _EnterTree()
	{ 
		_log = new GodotLogger(_logLevel);
		_map = GameMapFactory.CreateHexMap(_mapView.TileMap);
		_game = GameFactory.Create(_log, _map, _settings);

		var match = _game.GetComponent<Match>();
		var townCenters = _map.SearchTiles(t => t.Terrain == TileTerrain.TownCenter);

		// Set town centers based on map
		// TODO: Seems a bit hacky?
		match.LocalPlayer.TownCenter = townCenters.Single(tc => _mapView.IsHighlighted(tc.Position, HighlightLayer.BlueTeam));
		match.EnemyPlayer.TownCenter = townCenters.Single(tc => _mapView.IsHighlighted(tc.Position, HighlightLayer.RedTeam));
		
		// TODO: Formalize each player's zone of influence, so we can get it from outside MapView
		var reachable = _map.GetReachable(match.LocalPlayer.TownCenter.Position, 1).Select(t => t.Position);
		_mapView.HighlightTiles(reachable, HighlightLayer.BlueTeam);
		
		var reachable2 = _map.GetReachable(match.EnemyPlayer.TownCenter.Position, 1).Select(t => t.Position);
		_mapView.HighlightTiles(reachable2, HighlightLayer.RedTeam);
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
