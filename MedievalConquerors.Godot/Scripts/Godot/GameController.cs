using System.Linq;
using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Godot.Resources;

namespace MedievalConquerors.Godot;

public partial class GameController : Node
{
	[Export] private LogLevel _logLevel;
	
	[Export] private Match _match;
	[Export] private TileMap _tileMap;
	[Export] private GameSettings _settings;
	
	private Game _game;
	private ILogger _log;
	private IGameBoard _board;

	public override void _EnterTree()
	{ 
		_log = new GodotLogger(_logLevel);
		_board = GameBoardFactory.Create(_tileMap);
		_game = GameFactory.Create(_log, _match, _board, _settings);
	}

	public override void _Ready()
	{
		_game.Awake();
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
