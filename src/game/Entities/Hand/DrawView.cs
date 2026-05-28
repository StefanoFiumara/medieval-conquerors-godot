using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Entities.Cards;

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
		var drawAction = (DrawCardsAction) action;
		var prevHandCount =  _hand.Cards.Count;

		// TODO: Draw animation for opposite player?
		if (drawAction.TargetPlayerId != Match.LOCAL_PLAYER_ID) yield break;
		yield return true;
		if (drawAction.DrawnCards.Count == 0) yield break;

		if (prevHandCount > 0)
			_hand.ArrangeHandTween(totalCount: prevHandCount + drawAction.DrawnCards.Count);

		var sequence = CreateTween().SetParallel();
		List<CardView> createdViews = [];
		for (var i = 0; i < drawAction.DrawnCards.Count; i++)
		{
			var card = drawAction.DrawnCards[i];
			var cardView = _hand.CreateCardView(card, HandView.DeckPosition);
			cardView.ZIndex = 10 + i + prevHandCount;
			createdViews.Add(cardView);

			var tween = CreateTween().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out).SetParallel();

			var delay = i * TWEEN_DURATION * 0.25f;
			var (targetPosition, targetRotation, targetScale) = _hand.CalculateViewPosition(i + prevHandCount, prevHandCount + drawAction.DrawnCards.Count);

			tween.TweenInterval(delay);
			tween.Chain().TweenProperty(cardView, "position", targetPosition, TWEEN_DURATION);
			tween.TweenProperty(cardView, "rotation", targetRotation, TWEEN_DURATION);
			tween.TweenProperty(cardView, "scale", targetScale, TWEEN_DURATION);

			sequence.TweenSubtween(tween);
		}

		sequence.Chain().TweenCallback(Callable.From(() =>
		{
			foreach (var view in createdViews)
				_hand.AddCardView(view);
		}));

		while (sequence.IsRunning())
			yield return null;
	}

	private IEnumerator DiscardCardsAnimation(IGame game, GameAction action)
	{
		const double TWEEN_DURATION = 0.4;
		var discardAction = (DiscardCardsAction) action;

		yield return true;

		var discardedViews = _hand.Cards.Where(v => discardAction.CardsToDiscard.Contains(v.Card)).Reverse().ToList();
		if (discardedViews.Count == 0)
			yield break;

		var sequence = CreateTween().SetParallel();

		for (var i = 0; i < discardedViews.Count; i++)
		{
			var cardView = discardedViews[i];
			_hand.Cards.Remove(cardView);
			cardView.RemoveHighlight();

			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In).SetParallel();
			tween.TweenInterval(i * TWEEN_DURATION * 0.25f);
			tween.Chain().TweenProperty(cardView, "position", HandView.DiscardPosition, TWEEN_DURATION);
			tween.TweenProperty(cardView, "rotation", Mathf.Pi / 4, TWEEN_DURATION);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.3f, TWEEN_DURATION);
			tween.Chain().TweenCallback(Callable.From(cardView.QueueFree));

			sequence.TweenSubtween(tween);
		}

		while(sequence.IsRunning())
			yield return null;

		_hand.ArrangeHandTween();
	}

	private IEnumerator BanishCardsAnimation(IGame game, GameAction action)
	{
		const double TWEEN_DURATION = 0.2;

		yield return true;

		var banishAction = (BanishCardsAction) action;
		var banishedViews = _hand.Cards.Where(v => banishAction.CardsToBanish.Contains(v.Card)).Reverse().ToList();

		if (banishedViews.Count == 0)
			yield break;

		var sequence = CreateTween().SetParallel();
		for (var i = 0; i < banishedViews.Count; i++)
		{
			var cardView = banishedViews[i];
			_hand.Cards.Remove(cardView);
			cardView.RemoveHighlight();

			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In).SetParallel();
			tween.TweenInterval(i * TWEEN_DURATION * 0.25f);
			tween.Chain().TweenProperty(cardView, "position", cardView.Position + Vector2.Up * 60f, TWEEN_DURATION);
			tween.TweenProperty(cardView, "rotation", 0f, TWEEN_DURATION);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.3f, TWEEN_DURATION);
			tween.TweenProperty(cardView, "modulate:a", 0f, TWEEN_DURATION);
			tween.Chain().TweenCallback(Callable.From(cardView.QueueFree));

			sequence.TweenSubtween(tween);
		}

		while(sequence.IsRunning())
			yield return null;

		_hand.ArrangeHandTween();
	}
}
