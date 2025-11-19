using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Entities.Cards;

namespace MedievalConquerors.Entities.Hand;

/// <summary>
/// Connects animations for cards being created by other card effects
/// </summary>
public partial class CardCreationView : Node2D, IGameComponent
{
	private HandView _hand;
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
		_events.Subscribe<CreateCardAction>(GameEvent.Prepare<CreateCardAction>(), OnPrepareCreateCard);
	}

	private void OnPrepareCreateCard(CreateCardAction action)  => action.PerformPhase.Viewer = CreateCardAnimation;

	private IEnumerator CreateCardAnimation(IGame game, GameAction action)
	{
		const double TWEEN_DURATION = 0.6;
		yield return true;

		var createAction = (CreateCardAction) action;

		const float SPACING = 230f;
		var center = _viewport.GetVisibleRect().GetCenter();

		List<Tween> tweens = [];
		// var cardViews = createAction.CreatedCards.Select(c => CreateCardView(c)).ToList();
		// var displayTween = DisplayCardsTween(cardViews);

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		for (var i = 0; i < createAction.CreatedCards.Count; i++)
		{
			var createdCard = createAction.CreatedCards[i];
			var cardView = _hand.CreateCardView(createdCard, Vector2.Zero);
			var offset = i - (createAction.CreatedCards.Count - 1) / 2.0f;
			cardView.GlobalPosition = center + Vector2.Right * offset * SPACING;
			cardView.Scale = Vector2.Zero;
			tween.Chain().TweenInterval(0.3);
			tween.TweenProperty(cardView, "modulate:a", 1, TWEEN_DURATION).From((Variant.From(0)));
			tween.TweenProperty(cardView, "scale", Vector2.One, TWEEN_DURATION);

			if (createAction.TargetZone != Zone.Hand)
			{
				_hand.Cards.Remove(cardView);

				var subTween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
				var targetPosition = createAction.TargetZone == Zone.Deck
					? HandView.DeckPosition
					: HandView.DiscardPosition;

				var targetRotation = createAction.TargetZone == Zone.Deck
					? Mathf.Pi / 4
					: -Mathf.Pi / 4;

				subTween.TweenInterval(TWEEN_DURATION * i);
				subTween.Chain().TweenProperty(cardView, "position", targetPosition, TWEEN_DURATION / 2);
				subTween.TweenProperty(cardView, "rotation", targetRotation, TWEEN_DURATION / 2);
				subTween.Chain().TweenCallback(Callable.From(() =>
				{
					cardView.QueueFree();
				}));
				subTween.Pause();
				tweens.Add(subTween);
			}
		}

		tween.Chain().TweenInterval(0.5);
		while (tween.IsRunning()) yield return null;

		foreach (var subTween in tweens) subTween.Play();
		while (tweens.Any(t => t.IsRunning())) yield return null;
		if (createAction.TargetZone == Zone.Hand)
		{
			var handTweens = _hand.ArrangeHandTween();
			while (handTweens.Any(t => t.IsRunning())) yield return null;
		}
	}

	// TODO: Use helper tweens to compose CreateCardAnimation and make it shorter
	private Tween DisplayCardTween(CardView card, double duration = 0.4) => DisplayCardsTween([card], duration);
	private Tween DisplayCardsTween(List<CardView> cards, double duration = 0.4)
	{
		const float SPACING = 230f;

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		for (var i = 0; i < cards.Count; i++)
		{
			var card = cards[i];
			var offset = i - (cards.Count - 1) / 2.0f;
			tween.TweenCallback(Callable.From(() =>
			{
				var center = _viewport.GetVisibleRect().GetCenter();

				card.GlobalPosition = center + Vector2.Right * offset * SPACING;
				card.Scale = Vector2.Zero;
			}));

			tween.Chain().TweenInterval(duration * 0.5);
			tween.TweenProperty(card, "modulate:a", 1, duration).From((Variant.From(0)));
			tween.TweenProperty(card, "scale", Vector2.One, duration);
		}

		return tween;
	}
}
