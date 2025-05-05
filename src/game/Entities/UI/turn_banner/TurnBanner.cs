using System.Collections;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Screens;

namespace MedievalConquerors.Entities.UI.turn_banner;

public partial class TurnBanner : Control
{
	[Export] private GameController _gameController;

	private ColorRect _background;
	private RichTextLabel _turnLabel;

	private Game _game;
	private EventAggregator _events;

	public override void _Ready()
	{
		// TODO: Fix banner sizing on all resolutions
		_game = _gameController.Game;
		_background = GetNode<ColorRect>("%background");
		_turnLabel = GetNode<RichTextLabel>("%turn_text");

		_background.Scale = new Vector2(0, 1);
		_turnLabel.Modulate = new Color(_turnLabel.Modulate, 0f);

		_events = _game.GetComponent<EventAggregator>();
		_events.Subscribe<BeginTurnAction>(GameEvent.Prepare<BeginTurnAction>(), OnPrepareBeginTurnAction);
	}

	private void OnPrepareBeginTurnAction(BeginTurnAction action)
	{
		action.PerformPhase.Viewer = TweenTurnBanner;
	}

	private IEnumerator TweenTurnBanner(IGame game, GameAction action)
	{
		const float tweenDuration = 0.5f;
		const float middleDelay = 0.3f;

		var turnAction = (BeginTurnAction)action;
		var labelText = turnAction.PlayerId == Match.LocalPlayerId ? "Your Turn" : "Enemy Turn";

		_turnLabel.Text = labelText.Center();

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();

		tween.TweenProperty(_background, "scale", Vector2.One, tweenDuration);
		tween.TweenProperty(_turnLabel, "modulate:a", 1f, tweenDuration);
		tween.Chain().TweenCallback(Callable.From(() => _background.PivotOffset = _background.Size));

		tween.TweenProperty(_background, "scale", new Vector2(0f, 1f), tweenDuration).SetDelay(middleDelay);
		tween.TweenProperty(_turnLabel, "modulate:a", 0f, tweenDuration).SetDelay(middleDelay);;
		tween.Chain().TweenCallback(Callable.From(() => _background.PivotOffset = Vector2.Zero));

		while (tween.IsRunning())
			yield return null;
	}
}
