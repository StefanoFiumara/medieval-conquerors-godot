using System.Collections;
using System.Collections.Generic;
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

	public IGame Game { get; set; }

	public override void _EnterTree()
	{
		_hand = GetParent<HandView>();
		_hand.Game.AddComponent(this);
		_events = Game.GetComponent<EventAggregator>();
	}

	public override void _Ready()
	{
		_events.Subscribe<CreateCardAction>(GameEvent.Prepare<CreateCardAction>(), OnPrepareCreateCard);
	}

	private void OnPrepareCreateCard(CreateCardAction action)  => action.PerformPhase.Viewer = CreateCardAnimation;

	// TODO: Is it possible to split up this function into subtweens, to make things easier?
	private IEnumerator CreateCardAnimation(IGame game, GameAction action)
	{
		const double TWEEN_DURATION = 0.45;
		const float SPACING = 250f;

		var createAction = (CreateCardAction) action;
		if (createAction.TargetPlayerId != Match.LOCAL_PLAYER_ID) yield break;
		yield return true;
		if(createAction.CreatedCards.Count == 0) yield break;

		var center = GetViewport().GetVisibleRect().GetCenter();
		var sequence = CreateTween().SetParallel();
		var arrangeSequence = CreateTween().SetParallel();
		List<CardView> createdViews = [];

		for (var i = 0; i < createAction.CreatedCards.Count; i++)
		{
			var createdCard = createAction.CreatedCards[i];

			var cardView = _hand.CreateCardView(createdCard);
			cardView.ZIndex = 10 + _hand.Cards.Count + i;
			var positionOffset = i - (createAction.CreatedCards.Count - 1) / 2.0f;
			cardView.GlobalPosition = center + Vector2.Right * positionOffset * SPACING;
			cardView.Scale = Vector2.Zero;
			createdViews.Add(cardView);

			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
			tween.Chain().TweenInterval(0.3);
			tween.TweenProperty(cardView, "modulate:a", 1, TWEEN_DURATION).From((Variant.From(0)));
			tween.TweenProperty(cardView, "scale", Vector2.One, TWEEN_DURATION);

			sequence.TweenSubtween(tween);

			var arrangeTween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In).SetParallel();

			var (targetPosition, targetRotation, _) = _hand.CalculateViewPosition(_hand.Cards.Count + i,
				_hand.Cards.Count + createAction.CreatedCards.Count);

			targetPosition = createAction.TargetZone switch
			{
				Zone.Deck => HandView.DeckPosition,
				Zone.Discard => HandView.DiscardPosition,
				_ => targetPosition
			};

			targetRotation = createAction.TargetZone switch
			{
				Zone.Deck => Mathf.Pi / 4,
				Zone.Discard => -Mathf.Pi / 4,
				_ => targetRotation
			};

			arrangeTween.TweenInterval(0.3f * TWEEN_DURATION * i);
			arrangeTween.Chain().TweenProperty(cardView, "position", targetPosition, TWEEN_DURATION * 0.5f);
			arrangeTween.TweenProperty(cardView, "rotation", targetRotation, TWEEN_DURATION * 0.5f);
			if(createAction.TargetZone != Zone.Hand)
				arrangeTween.Chain().TweenCallback(Callable.From(cardView.QueueFree));

			arrangeSequence.TweenSubtween(arrangeTween);
		}

		sequence.Chain().TweenInterval(0.5);
		sequence.Chain().TweenSubtween(arrangeSequence);
		if(createAction.TargetZone == Zone.Hand)
			sequence.TweenSubtween(_hand.ArrangeHandTween(totalCount: _hand.Cards.Count + createdViews.Count));

		sequence.Chain().TweenCallback(Callable.From(() =>
		{
			if (createAction.TargetZone == Zone.Hand)
			{
				foreach (var view in createdViews)
					_hand.AddCardView(view);
			}
		}));

		while (sequence.IsRunning()) yield return null;
	}
}
