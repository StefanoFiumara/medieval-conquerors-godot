using System;
using System.Collections;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Entities.Cards;
using MedievalConquerors.Entities.Maps;

namespace MedievalConquerors.Entities.Hand;

/// <summary>
/// Connects animations for cards being played to the board
/// </summary>
public partial class PlayCardsView : Node2D, IGameComponent
{
	private HandView _hand;
	private MapView _map;

	private EventAggregator _events;
	private Viewport _viewport;

	public IGame Game { get; set; }

	public override void _EnterTree()
	{
		_hand = GetParent<HandView>();
		_hand.Game.AddComponent(this);
		_events = Game.GetComponent<EventAggregator>();
		_viewport = GetViewport();
	}

	public override void _Ready()
	{
		_map = Game.GetComponent<MapView>();
		_events.Subscribe<PlayCardAction>(GameEvent.Prepare<PlayCardAction>(), OnPreparePlayCard);
	}

	private void OnPreparePlayCard(PlayCardAction action)      => action.PerformPhase.Viewer = PlayCardAnimation;

	private IEnumerator PlayCardAnimation(IGame game, GameAction action)
	{
		yield return true;
		var playAction = (PlayCardAction) action;

		if (playAction.CardToPlay.Owner.Id == Match.LOCAL_PLAYER_ID)
		{
			var cardView = _hand.Cards.Single(c => c.Card == playAction.CardToPlay);
			var playCardTween = PlayCardTween(cardView, playAction.TargetTile);
			playCardTween.Chain().TweenCallback(Callable.From((Action)(() => _hand.ArrangeHandTween())));
		}
	}

	private Tween PlayCardTween(CardView cardView, Vector2I targetTile)
	{
		// TODO: verify we no longer need this
		// if (cardView == null)
		// {
		//     var nullTween = CreateTween();
		//     nullTween.TweenCallback(Callable.From(() => { }));
		//     return nullTween;
		// }

		return cardView.Card.Data.CardType == CardType.Technology
			? CenterAndFadeAwayTween(cardView)
			: PlayOnTileTween(cardView, targetTile);
	}

	private Tween PlayOnTileTween(CardView card, Vector2I tile, double duration = 0.5)
	{
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		var targetPosition = _map.GetTileGlobalPosition(tile);
		tween.TweenProperty(card, "position", ToLocal(targetPosition), duration);
		tween.TweenProperty(card, "scale", Vector2.One * 0.2f, duration).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(card, "modulate:a", 0f, duration);

		tween.Chain().TweenCallback(Callable.From(() =>
		{
			_hand.Cards.Remove(card);
			card.QueueFree();
		}));

		return tween;
	}

	private Tween CenterAndFadeAwayTween(CardView card, double duration = 0.5)
	{
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();

		tween.TweenProperty(card, "global_position", _viewport.GetVisibleRect().GetCenter(), duration);
		tween.TweenProperty(card, "rotation", 0, duration);
		tween.TweenProperty(card, "scale", Vector2.One, duration).SetEase(Tween.EaseType.Out);
		tween.Chain().TweenInterval(0.4f);
		tween.TweenProperty(card, "scale", Vector2.One * 1.5f, duration);
		tween.TweenProperty(card, "modulate:a", 0f, duration);

		tween.Chain().TweenCallback(Callable.From(() =>
		{
			_hand.Cards.Remove(card);
			card.QueueFree();
		}));

		return tween;
	}
}
