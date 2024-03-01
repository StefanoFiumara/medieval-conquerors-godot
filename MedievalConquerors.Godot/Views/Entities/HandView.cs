using System.Collections;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Views.Main;

namespace MedievalConquerors.Views.Entities;

public partial class HandView : Node2D
{
	// TODO: Difference HandWidth based on card count? experiment with this
	private const float MaxHandWidth = 750f;
	private const float HandHeight = 95f;
	private const int MaxHandCount = 15;
	
	private readonly List<CardView> _cards = new();
	private PackedScene _cardScene;

	private Game _game;
	private EventAggregator _events;
	
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;

	public override void _Ready()
	{
		_cardScene = GD.Load<PackedScene>("res://Views/Entities/Card.tscn");
		
		_game = GetParent<GameController>().Game;
		_events = _game.GetComponent<EventAggregator>();
		
		_events.Subscribe<DrawCardsAction>(GameEvent.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
	}

	private void OnPrepareDrawCards(DrawCardsAction action)
	{
		action.PerformPhase.Viewer = DrawCardsAnimation;
	}

	private IEnumerator DrawCardsAnimation(IGame game, GameAction action)
	{
		yield return true;

		var drawAction = (DrawCardsAction) action;

		// TODO: Draw animation for opposite player?
		if (drawAction.TargetPlayerId != 0) yield break;

		foreach (var card in drawAction.DrawnCards)
		{
			AddCard(card); // TODO send card as param for view
			var animation = GetHandAnimation();
		
			while (animation.IsRunning())
				yield return null;
		}
	}

	private Tween GetHandAnimation()
	{
		var tween = CreateTween()
				.SetTrans(Tween.TransitionType.Sine)
				.SetParallel();

		foreach (var card in _cards)
		{
			var (targetPosition, targetRotation) = CalculateHandPosition(card);
			
			tween.TweenProperty(card, "position", targetPosition, 0.3);
			tween.TweenProperty(card, "rotation", targetRotation, 0.3);
			//tween.Chain();
		}

		return tween;
	}

	private (Vector2 position, float rotation) CalculateHandPosition(CardView card)
	{
		var ratio = CalculateRatio(card);
		
		var xOffset = _spreadCurve.Sample(ratio) * (MaxHandWidth * (_cards.Count / 12f));
		var yOffset = -_heightCurve.Sample(ratio) * (HandHeight * (_cards.Count / 10f));
		var position = new Vector2(xOffset, yOffset);
		
		var rotation = -_rotationCurve.Sample(ratio) * (0.25f * (_cards.Count / 10f));

		return (position, rotation);
	}

	// TEMP: Testing different hand counts
	public override void _Input(InputEvent inputEvent)
	{
		// Only respond to KeyDown events
		if (inputEvent.IsEcho()) return;
		
		if (inputEvent is InputEventKey e && e.IsPressed())
		{
			if (e.Keycode == Key.Left)
			{
				RemoveCard();
				GetHandAnimation();
			}
			else if (e.Keycode == Key.Right)
			{
				AddCard(null);
				GetHandAnimation();
			}
		}
	}

	private void AddCard(Card card)
	{
		var cardView = _cardScene.Instantiate<CardView>();
			
		// Spawn card offscreen, to be animated in by SetCardPositions
		cardView.Position = (Vector2.Left * 1200) + (Vector2.Down * 300);
			
		_cards.Add(cardView);
		AddChild(cardView);
		
		// TEMP: card should never be null.
		if(card != null)
			cardView.Initialize(card);
	}

	private void RemoveCard()
	{
		// TEMP: For testing hand animations
		if (_cards.Count == 0) return;

		var card = _cards[0];
		_cards.Remove(card);
		card.QueueFree();
	}
	
	private float CalculateRatio(CardView card)
	{
		var ratio = 0.5f;
		if (_cards.Count > 1)
			ratio = _cards.IndexOf(card) / (float) (_cards.Count - 1);

		return ratio;
	}
}
