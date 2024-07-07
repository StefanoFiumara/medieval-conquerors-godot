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
	private const int BackgroundHeight = 140;
	private const int BannerFontSize = 110; 
	
	[Export] private ColorRect _background;
	[Export] private RichTextLabel _turnLabel;
	
	private Game _game;
	private EventAggregator _events;

	private Viewport _viewport;
	
	public override void _Ready()
	{
		_game = GetParent<GameController>().Game;
		_events = _game.GetComponent<EventAggregator>();
		
		_events.Subscribe<ChangeTurnAction>(GameEvent.Prepare<ChangeTurnAction>(), OnPrepareChangeTurnAction);
	}

	public override void _EnterTree()
	{
		_viewport = GetViewport();
		CalculateViewPosition();
		_viewport.SizeChanged += CalculateViewPosition;
	}

	private void CalculateViewPosition()
	{
		var visibleRect = _viewport.GetVisibleRect();
		var proportion = visibleRect.Size / ViewConstants.ReferenceResolution;

		_background.Size = new Vector2(visibleRect.Size.X, BackgroundHeight * proportion.Y);
		_background.Position = new Vector2(0, visibleRect.Size.Y * 0.5f - (BackgroundHeight * 0.5f));
		
		_turnLabel.AddThemeFontSizeOverride("normal_font_size", (int)(BannerFontSize * proportion.Y));
		_turnLabel.Size = _background.Size;
		_turnLabel.Position = _background.Position;
		
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
	}
}
