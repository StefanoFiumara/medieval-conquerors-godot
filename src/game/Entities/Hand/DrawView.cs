using System.Collections;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Screens;

namespace MedievalConquerors.Entities.Hand;

public partial class DrawView : Node2D, IGameComponent
{
	private EventAggregator _events;
	private HandView _hand;

	public IGame Game { get; set; }

	public override void _EnterTree()
	{
		GetParent<HandView>().Game.AddComponent(this);
		_hand = GetParent<HandView>();
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

		// CalculatePreviewBoundaries();
	}

	private IEnumerator DiscardCardsAnimation(IGame game, GameAction action)
	{
		const double tweenDuration = 0.25;
		var discardAction = (DiscardCardsAction) action;

		yield return true;

		discardAction.CardsToDiscard.Reverse();
		foreach (var card in discardAction.CardsToDiscard)
		{
			var cardView = _hand.Cards.SingleOrDefault(c => c.Card == card);
			if (cardView == null) continue;

			cardView.RemoveHighlight();

			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();

			tween.TweenProperty(cardView, "position", HandView.DiscardPosition, tweenDuration);
			tween.TweenProperty(cardView, "rotation", Mathf.Pi / 4, tweenDuration);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.3f, tweenDuration);

			while (tween.IsRunning()) yield return null;
			_hand.Cards.Remove(cardView);
			cardView.QueueFree();
		}

		var arrangeTweens = _hand.ArrangeHandTween();
		while (arrangeTweens.Any(t => t.IsRunning())) yield return null;
	}
}
