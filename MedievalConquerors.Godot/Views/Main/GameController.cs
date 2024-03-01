using System.Linq;
using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.GameData.GameSettings;
using MedievalConquerors.Views.Maps;

namespace MedievalConquerors.Views.Main;

public partial class GameController : Node
{
	[Export] private LogLevel _logLevel;
	[Export] private GameSettings _settings;
	
	// TODO: Load different maps through this resource
	[Export] private MapView _gameMap;
	
	
	private Game _game;
	private ILogger _log;
	private IGameBoard _board;

	public Game Game => _game;

	public override void _EnterTree()
	{ 
		_log = new GodotLogger(_logLevel);
		_board = GameBoardFactory.CreateHexBoard(_gameMap);
		_game = GameFactory.Create(_log, _board, _settings);
		
		// TEMP: testing ranges
		Visualize(_range);
	}

	public override void _Ready()
	{
		_game.Awake();
		_game.Perform(new BeginGameAction(0));
	}

	// TEMP: Testing variable ranges
	private int _range = 1;
	private void Visualize(int range)
	{
		_gameMap.Clear(HighlightLayer.RangeVisualizer);
		var townCenters = _board.SearchTiles(t => t.Terrain == TileTerrain.TownCenter);
		foreach (var tc in townCenters)
		{
			var reachable = _board.GetReachable(tc.Position, range).Select(t => t.Position);
			_gameMap.Visualize(reachable);
		}
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
