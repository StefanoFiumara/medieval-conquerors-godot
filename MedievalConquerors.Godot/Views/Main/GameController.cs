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
	private HexMap _map;

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
		
		// TODO: this should updated dynamically as Player's Influence Range changes, perhaps in MapView when responding to some actions.
		var tilesInfluencedLocal = _map.GetReachable(match.LocalPlayer.TownCenter.Position, match.LocalPlayer.InfluenceRange);
		_mapView.HighlightTiles(tilesInfluencedLocal, HighlightLayer.BlueTeam);
		
		var tilesInfluencedEnemy = _map.GetReachable(match.EnemyPlayer.TownCenter.Position, match.EnemyPlayer.InfluenceRange);
		_mapView.HighlightTiles(tilesInfluencedEnemy, HighlightLayer.RedTeam);
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
