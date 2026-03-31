using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Entities.Hand;

/// <summary>
/// Connects animations for cards entering and exiting the hand
/// TODO: Better name for this view since this is handling both Draw AND Discard animations?
/// </summary>
public partial class DrawView : Node2D, IGameComponent
{
	private EventAggregator _events;
	private HandView _hand;

	public IGame Game { get; set; }

	public override void _EnterTree()
	{
		_hand = GetParent<HandView>();

		_hand.Game.AddComponent(this);
		_events = Game.GetComponent<EventAggregator>();
	}

	public override void _Ready()
	{
		_events.Subscribe<DrawCardsAction>(GameEvent.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
		_events.Subscribe<DiscardCardsAction>(GameEvent.Prepare<DiscardCardsAction>(), OnPrepareDiscardCards);
		_events.Subscribe<BanishCardsAction>(GameEvent.Prepare<BanishCardsAction>(), OnPrepareBanishCards);
	}

	private void OnPrepareDrawCards(DrawCardsAction action)       => action.PerformPhase.Viewer = DrawCardsAnimation;
	private void OnPrepareDiscardCards(DiscardCardsAction action) => action.PerformPhase.Viewer = DiscardCardsAnimation;
	private void OnPrepareBanishCards(BanishCardsAction action)   => action.PerformPhase.Viewer = BanishCardsAnimation;

	private IEnumerator DrawCardsAnimation(IGame game, GameAction action)
	{
		const double TWEEN_DURATION = 0.6;

		int prevHandCount = _hand.Cards.Count;
		yield return true;

		var drawAction = (DrawCardsAction) action;
		// TODO: Draw animation for opposite player?
		if (drawAction.TargetPlayerId != Match.LOCAL_PLAYER_ID) yield break;

		foreach (var card in drawAction.DrawnCards)
			_hand.CreateCardView(card, HandView.DeckPosition);

		if (prevHandCount > 0)
		{
			_hand.ArrangeHandTween(max: prevHandCount);
		}

		List<Tween> tweens = [];
		for (var i = 0; i < drawAction.DrawnCards.Count; i++)
		{
			var card = drawAction.DrawnCards[i];
			var cardView = _hand.Cards.SingleOrDefault(c => c.Card == card);

			var tween = CreateTween().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out).SetParallel();

			var delay = i * TWEEN_DURATION * 0.25f;
			var (targetPosition, targetRotation) = _hand.GetCardPosition(cardView);

			tween.TweenInterval(delay);
			tween.Chain().TweenProperty(cardView, "position", targetPosition, TWEEN_DURATION);
			tween.TweenProperty(cardView, "rotation", targetRotation, TWEEN_DURATION);
			tween.TweenProperty(cardView, "scale", Vector2.One, TWEEN_DURATION);

			tweens.Add(tween);
		}

		if(tweens.Count > 0)
			while (tweens[^1].IsRunning()) yield return null;

		// TODO: may need to run this before and after the draw animation, depending on how it looks
		var handTweens = _hand.ArrangeHandTween();
		//while (handTweens.Any(t => t.IsRunning())) yield return null;


	}

	private IEnumerator DiscardCardsAnimation(IGame game, GameAction action)
	{
		const double TWEEN_DURATION = 0.4;
		var discardAction = (DiscardCardsAction) action;

		yield return true;

		var discardedViews = _hand.Cards.Where(v => discardAction.CardsToDiscard.Contains(v.Card)).Reverse().ToList();
		List<Tween> tweens = [];
		for (var i = 0; i < discardedViews.Count; i++)
		{
			var cardView = discardedViews[i];
			cardView.RemoveHighlight();
			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In).SetParallel();

			var delay = i * TWEEN_DURATION * 0.25f;
			tween.TweenInterval(delay);
			tween.Chain().TweenProperty(cardView, "position", HandView.DiscardPosition, TWEEN_DURATION);
			tween.TweenProperty(cardView, "rotation", Mathf.Pi / 4, TWEEN_DURATION);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.3f, TWEEN_DURATION);
			tween.TweenProperty(cardView, "modulate:a", 0f, TWEEN_DURATION);
			tween.Chain().TweenCallback(Callable.From(() =>
			{
				_hand.Cards.Remove(cardView);
				cardView.QueueFree();
			}));

			tweens.Add(tween);
		}

		if (tweens.Count > 0)
		{
			while (tweens[^1].IsRunning()) yield return null;

			var arrangeTween = _hand.ArrangeHandTween();
			while (arrangeTween.IsRunning()) yield return null;
		}
	}

	private IEnumerator BanishCardsAnimation(IGame game, GameAction action)
	{
		const double TWEEN_DURATION = 0.4;
		var discardAction = (BanishCardsAction) action;

		yield return true;

		var banishedViews = _hand.Cards.Where(v => discardAction.CardsToBanish.Contains(v.Card)).Reverse().ToList();
		List<Tween> tweens = [];
		for (var i = 0; i < banishedViews.Count; i++)
		{
			var cardView = banishedViews[i];
			cardView.RemoveHighlight();
			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In).SetParallel();

			var delay = i * TWEEN_DURATION * 0.25f;
			tween.TweenInterval(delay);
			tween.Chain().TweenProperty(cardView, "position", cardView.Position + Vector2.Up * 60f, TWEEN_DURATION);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.3f, TWEEN_DURATION);
			tween.TweenProperty(cardView, "modulate:a", 0f, TWEEN_DURATION);
			tween.Chain().TweenCallback(Callable.From(() =>
			{
				_hand.Cards.Remove(cardView);
				cardView.QueueFree();
			}));

			tweens.Add(tween);
		}

		if (tweens.Count > 0)
		{
			while (tweens[^1].IsRunning()) yield return null;

			var arrangeTween = _hand.ArrangeHandTween();
			while (arrangeTween.IsRunning()) yield return null;
		}
	}
}
