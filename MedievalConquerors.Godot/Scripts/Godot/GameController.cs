using System.Collections;
using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Godot.Resources;

namespace MedievalConquerors.Godot;

public partial class GameController : Node
{
	[Export] private Match _match;
	[Export] private GameSettings _settings;
	[Export] private Sprite2D _testObject;
	[Export] private LogLevel _logLevel;
	
	private Game _game;
	private ILogger _log;
	private IEventAggregator _events;
	
	public override void _EnterTree()
	{ 
		_log = new GodotLogger(_logLevel);
		_game = GameFactory.Create(_log, _match, _settings);
		_events = _game.GetComponent<EventAggregator>();
		
		
		// TEMP: Testing Game Action
		_events.Subscribe<GameAction>(GameEvent.Perform<GameAction>(), OnPerformGameAction);
		_events.Subscribe<GameAction, ActionValidatorResult>(GameEvent.Validate<GameAction>(), OnValidateGameAction);
	}

	public override void _Ready()
	{
		_game.Awake();
		
		// TEMP: Testing game action
		var action = new GameAction();
		action.PerformPhase.Viewer = TestAnimation;
		_game.Perform(action);
	}

	private void OnValidateGameAction(GameAction source, ActionValidatorResult validatorResult)
	{
		// validatorResult.Invalidate("Game Action Not Allowed");
	}

	// TEMP: Testing perform action callbacks
	private void OnPerformGameAction(GameAction sender)
	{
		GD.PrintRich("GameController\t======> PerformedAction".Red());
	}
	
	// TEMP: Testing animations via GameAction callbacks
	private float _animationSpeed = 5;
	private Vector2 _targetPosition = new Vector2(300, 900);
	private IEnumerator TestAnimation(IGame game, GameAction action)
	{
		yield return true;
		var tween = CreateTween().SetTrans(Tween.TransitionType.Cubic);
		tween.TweenProperty(_testObject, new NodePath(Node2D.PropertyName.Position), _targetPosition, 3);

		while (tween.IsRunning())
		{
			yield return null;
		}
		
		GD.PrintRich("GameController\t======> Tween Finished".Red());
	}

	public override void _Process(double elapsed)
	{
		_game.Update();
	}

	public override void _ExitTree()
	{
		_game.Destroy();
		// TEMP: Testing game action
		_events.Unsubscribe(GameEvent.Perform<GameAction>(), OnPerformGameAction);
		_events.Unsubscribe(GameEvent.Validate<GameAction>(), OnValidateGameAction);
	}
}
