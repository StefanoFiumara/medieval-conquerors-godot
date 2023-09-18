using System.Linq;
using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Godot.Resources;

namespace MedievalConquerors.Godot;

public partial class GameController : Node
{
	[Export] private LogLevel _logLevel;
	[Export] private GameSettings _settings;
	[Export] private Match _match;
	[Export] private TileMapHighlighter _tileMap;
	
	
	private Game _game;
	private ILogger _log;
	private IGameBoard _board;

	public override void _EnterTree()
	{ 
		_log = new GodotLogger(_logLevel);
		_board = GameBoardFactory.CreateHexBoard(_tileMap);
		_game = GameFactory.Create(_log, _match, _board, _settings);
		
		// TEMP: testing ranges
		Visualize(_range);
	}

	public override void _Ready()
	{
		_game.Awake();
	}

	// TEMP: Testing variable ranges
	private int _range = 1;
	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsEcho()) return;
		
		if (inputEvent is InputEventKey e && e.IsPressed())
		{
			if (e.Keycode == Key.Left)
			{
				_range = Mathf.Max(_range - 1, 1);
				Visualize(_range);
			}
			else if (e.Keycode == Key.Right)
			{
				_range = Mathf.Min(_range + 1, 4);
				Visualize(_range);
			}
		}
	}
	
	private void Visualize(int range)
	{
		_tileMap.Clear(HighlightLayer.RangeVisualizer);
		var townCenters = _board.SearchTiles(t => t.Terrain == TileTerrain.TownCenter);
		foreach (var tc in townCenters)
		{
			var reachable = _board.GetReachable(tc.Position, range).Select(t => t.Position);
			_tileMap.Visualize(reachable);
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
