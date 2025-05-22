using System.Collections;
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
	}

	private void OnPrepareDrawCards(DrawCardsAction action)       => action.PerformPhase.Viewer = DrawCardsAnimation;
	private void OnPrepareDiscardCards(DiscardCardsAction action) => action.PerformPhase.Viewer = DiscardCardsAnimation;

	private IEnumerator DrawCardsAnimation(IGame game, GameAction action)
	{
		yield return true;
		var drawAction = (DrawCardsAction) action;

		// TODO: Draw animation for opposite player?
		if (drawAction.TargetPlayerId != Match.LocalPlayerId) yield break;

		foreach (var card in drawAction.DrawnCards)
		{
			_hand.CreateCardView(card, HandView.DeckPosition);
			var tweens = _hand.ArrangeHandTween();
			while (tweens.Any(t => t.IsRunning())) yield return null;
		}
	}

	private IEnumerator DiscardCardsAnimation(IGame game, GameAction action)
	{
		const double tweenDuration = 0.25;
		var discardAction = (DiscardCardsAction) action;

		yield return true;

		var discardedViews = _hand.Cards.Where(v => discardAction.CardsToDiscard.Contains(v.Card)).Reverse().ToList();
		foreach (var cardView in discardedViews)
		{
			cardView.RemoveHighlight();

			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();

			tween.TweenProperty(cardView, "position", HandView.DiscardPosition, tweenDuration);
			tween.TweenProperty(cardView, "rotation", Mathf.Pi / 4, tweenDuration);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.3f, tweenDuration);
			tween.TweenProperty(cardView, "modulate:a", 0f, tweenDuration);
			tween.Chain().TweenCallback(Callable.From(() =>
			{
				_hand.Cards.Remove(cardView);
				cardView.QueueFree();
			}));

			// TODO: convert this into a tween with an interval so we can overlap the card animations a little bit
			while (tween.IsRunning()) yield return null;
		}

		var arrangeTweens = _hand.ArrangeHandTween();
		while (arrangeTweens.Any(t => t.IsRunning())) yield return null;
	}
}
