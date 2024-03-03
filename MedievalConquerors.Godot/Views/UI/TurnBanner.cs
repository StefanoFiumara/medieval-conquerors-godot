using System.Collections;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;
using MedievalConquerors.Views.Main;

namespace MedievalConquerors.Views.UI;

public partial class TurnBanner : Node2D
{
	private Game _game;
	private EventAggregator _events;

	private ColorRect _background;
	private RichTextLabel _turnLabel;

	public override void _Ready()
	{
		_background = GetNode<ColorRect>("background");
		_turnLabel = GetNode<RichTextLabel>("turn_text");

		_game = GetParent<GameController>().Game;
		_events = _game.GetComponent<EventAggregator>();
		
		_events.Subscribe<ChangeTurnAction>(GameEvent.Prepare<ChangeTurnAction>(), OnPrepareChangeTurnAction);

		_background.Scale = new Vector2(0, 1);
		_turnLabel.Modulate = new Color(_turnLabel.Modulate, 0f);
	}

	private void OnPrepareChangeTurnAction(ChangeTurnAction action)
	{
		action.PerformPhase.Viewer = TweenTurnBanner;
	}

	private IEnumerator TweenTurnBanner(IGame game, GameAction action)
	{
		const float tweenDuration = 0.5f;
		const float middleDelay = 0.3f;
		
		var turnAction = (ChangeTurnAction)action;
		var labelText = turnAction.NextPlayerId == Match.LocalPlayerId ? "Your Turn" : "Enemy Turn";

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
		
		yield return true;
	}
}