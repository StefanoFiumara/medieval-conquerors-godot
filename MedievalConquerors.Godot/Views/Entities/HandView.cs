using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.Utils;
using MedievalConquerors.Views.Main;

namespace MedievalConquerors.Views.Entities;

public partial class HandView : Node2D
{
	private const float MaxHandWidth = 750f;
	private const float HandHeight = 95f;
	private const int MaxHandCount = 13;
	private const int PreviewSectionHeight = 300;
	
	private readonly List<CardView> _cards = new();
	
	private int _previewXMin;
	private int _previewXMax;
	private int _previewSectionSize = 150;
	private int _hoveredIndex = -1;
	
	private Game _game;
	private EventAggregator _events;
	private Viewport _viewport;
	private readonly TweenTracker<CardView> _tweenTracker = new();
	
	[Export] private PackedScene _cardScene;
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;
	
	public override void _Ready()
	{
		_game = GetParent<GameController>().Game;
		_events = _game.GetComponent<EventAggregator>();
		
		
        
		_events.Subscribe<DrawCardsAction>(GameEvent.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
	}

	public override void _EnterTree()
	{
		_viewport = GetViewport();
		AnchorViewToScreen();
		_viewport.SizeChanged += AnchorViewToScreen;
	}

	public override void _ExitTree() => _viewport.SizeChanged -= AnchorViewToScreen;

	private void AnchorViewToScreen()
	{
		var viewportRect = _viewport.GetVisibleRect();
		Position = new Vector2(viewportRect.Size.X * 0.5f, viewportRect.Size.Y - 175f);
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
			CreateCardView(card);
			
			var tweens = TweenToHandPositions();
			while (tweens.Any(t => t.IsRunning()))
				yield return null;
		}

		CalculatePreviewBoundaries();
	}

	public override void _Process(double elapsed)
	{
		var mousePos = ToLocal(_viewport.GetMousePosition());
		var hovered = GetHoveredCardIndex(mousePos);

		if (hovered != _hoveredIndex)
		{
			if (hovered != -1)
			{
				TweenToPreviewPosition(hovered);
			}
			
			_hoveredIndex = hovered;
			TweenToHandPositions();
		}
	}

	private int GetHoveredCardIndex(Vector2 mousePos)
	{
		for (int i = 0; i < _cards.Count; i++)
		{
			// TODO: Re-enable debug drawing of these rects
			var sectionRect = new Rect2(
				x: _previewXMin + i * _previewSectionSize,
				y: -150,
				width: _previewSectionSize,
				height: PreviewSectionHeight
			);

			if (sectionRect.HasPoint(mousePos))
			{
				return i;
			}
		}

		return -1;
	}

	private Tween TweenToPreviewPosition(int index)
	{
		const double tweenDuration = 0.2;
		
		var card = _cards[index];
		card.ZIndex = 100;
		
		var tween = CreateTween()
			.SetTrans(Tween.TransitionType.Sine)
			.SetParallel();

		var (handPos, _) = CalculateHandPosition(card);
		tween.TweenProperty(card, "position", handPos + Vector2.Up * (120f + handPos.Y), tweenDuration);
		tween.TweenProperty(card, "rotation", 0, tweenDuration);
		tween.TweenProperty(card, "scale", Vector2.One * 1.3f, tweenDuration);
		
		_tweenTracker.TrackTween(tween, card);
		return tween;
	}

	private void CalculatePreviewBoundaries()
	{
		if (_cards.Count > 0)
		{
			_previewXMin = (int)(_cards[0].Position.X - 100 * _cards[0].Scale.X);
			_previewXMax = (int)(_cards[^1].Position.X + 100 * _cards[0].Scale.X);
			_previewSectionSize = (_previewXMax - _previewXMin) / _cards.Count;
		}
		else
		{
			_previewXMin = 0;
			_previewXMax = 0;
			_previewSectionSize = 0;
		}
	}

	private List<Tween> TweenToHandPositions()
	{
		const double tweenDuration = 0.3;
		var tweens = new List<Tween>();
		
		for (var i = 0; i < _cards.Count; i++)
		{
			// Do not animate hovered card.
			if (i == _hoveredIndex) continue;
			
			var card = _cards[i];
			card.ZIndex = i;
	
			var tween = CreateTween()
				.SetTrans(Tween.TransitionType.Sine)
				.SetParallel();
			
			var (targetPosition, targetRotation) = CalculateHandPosition(card);

			tween.TweenProperty(card, "position", targetPosition, tweenDuration);
			tween.TweenProperty(card, "rotation", targetRotation, tweenDuration);
			tween.TweenProperty(card, "scale", Vector2.One, tweenDuration);
			
			tweens.Add(tween);
			_tweenTracker.TrackTween(tween, card);
		}
		
		return tweens;
	}

	private (Vector2 position, float rotation) CalculateHandPosition(CardView card)
	{
		var ratio = 0.5f;
		if (_cards.Count > 1)
			ratio = _cards.IndexOf(card) / (float) (_cards.Count - 1);
		
		var xOffset = _spreadCurve.Sample(ratio) * (MaxHandWidth * (_cards.Count / 10f));
		var yOffset = -_heightCurve.Sample(ratio) * (HandHeight * (_cards.Count / 10f));
		var position = new Vector2(xOffset, yOffset);
		
		var rotation = -_rotationCurve.Sample(ratio) * (0.25f * (_cards.Count / 10f));

		return (position, rotation);
	}

	private void CreateCardView(Card card)
	{
		var cardView = _cardScene.Instantiate<CardView>();
			
		// Spawn card offscreen, to be animated in by TweenToHandPositions
		cardView.Position = (Vector2.Left * 1200) + (Vector2.Down * 300);
			
		_cards.Add(cardView);
		AddChild(cardView);
		
		cardView.Initialize(card);
	}
	
	public override void _Input(InputEvent inputEvent)
	{
		// Only respond to KeyDown/KeyUp events
		if (inputEvent.IsEcho()) return;

		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.IsReleased())
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left && _hoveredIndex != -1)
			{
				_events.Publish(InputSystem.ClickedEvent, _cards[_hoveredIndex].Card);
				_viewport.SetInputAsHandled();
			}
		}
	}
}
