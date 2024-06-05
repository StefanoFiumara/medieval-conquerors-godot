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
using MedievalConquerors.Views.Maps;

namespace MedievalConquerors.Views.Entities;

public partial class HandView : Node2D, IGameComponent
{
	private const float MaxHandWidth = 750f;
	private const float HandHeight = 95f;
	private const int PreviewSectionHeight = 350;
	
	private readonly List<CardView> _cards = new();
	
	private int _previewXMin;
	private int _previewXMax;
	private int _previewSectionSize = 150;
	private int _hoveredIndex = -1;
	
	private IGameSettings _settings;
	private EventAggregator _events;
	private Viewport _viewport;
	private readonly TweenTracker<CardView> _tweenTracker = new();
	
	[Export] private PackedScene _cardScene;
	
	// TODO: Do we need a more dynamic way to get a mapView reference? 
	[Export] private MapView _mapView;
	
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;
	
	public IGame Game { get; set; }
	
	public override void _Ready()
	{
		Game = GetParent<GameController>().Game;
		Game.AddComponent(this);
		
		_events = Game.GetComponent<EventAggregator>();
		_events.Subscribe<PlayCardAction>(GameEvent.Prepare<PlayCardAction>(), OnPreparePlayCard);
		_events.Subscribe<DrawCardsAction>(GameEvent.Prepare<DrawCardsAction>(), OnPrepareDrawCards);

		_settings = Game.GetComponent<IGameSettings>();
	}

	public override void _EnterTree()
	{
		_viewport = GetViewport();
		AnchorViewToScreen();
		_viewport.SizeChanged += AnchorViewToScreen;
	}

	public override void _ExitTree() => _viewport.SizeChanged -= AnchorViewToScreen;

	public override void _Process(double elapsed)
	{
		var mousePos = ToLocal(_viewport.GetMousePosition());
		var hovered = CalculateHoveredIndex(mousePos);

		if (hovered != _hoveredIndex)
		{
			if (hovered != -1)
			{
				TweenToPreviewPosition(hovered);
			}
			
			_hoveredIndex = hovered;
			TweenToHandPositions();

			// TODO: This might have to exclude cards that are actively being selected by the input system, how to integrate?
			// IDEA: Move to input system and expose _hoveredIndex? Or have a property for hovered CardView?
			for (int i = 0; i < _cards.Count; i++)
			{
				if(i == _hoveredIndex)
					_cards[_hoveredIndex].Highlight();
				else
					_cards[i].RemoveHighlight();
			}
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.IsReleased())
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left && _hoveredIndex != -1)
			{
				_events.Publish(InputSystem.ClickedEvent, _cards[_hoveredIndex], mouseEvent);
				_viewport.SetInputAsHandled();
			}
		}
	}

	public override void _Draw()
	{
		if (!_settings.DebugMode)
			return;
		
		for (int i = 0; i < _cards.Count; i++)
		{
			var sectionRect = new Rect2(
				x: _previewXMin + i * _previewSectionSize,
				y: -150,
				width: _previewSectionSize,
				height: PreviewSectionHeight
			);

			DrawRect(sectionRect, Colors.Magenta, false, 2);
		}
	}

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
		if (drawAction.TargetPlayerId != Match.LocalPlayerId) yield break;

		foreach (var card in drawAction.DrawnCards)
		{
			CreateCardView(card);
			
			var tweens = TweenToHandPositions();
			while (tweens.Any(t => t.IsRunning()))
				yield return null;
		}

		CalculatePreviewBoundaries();
		QueueRedraw();
	}

	private void OnPreparePlayCard(PlayCardAction action)
	{
		action.PerformPhase.Viewer = PlayCardAnimation;
	}

	private IEnumerator PlayCardAnimation(IGame game, GameAction action)
	{
		var playAction = (PlayCardAction) action;
		
		var tokenTween = _mapView.PlaceTokenAnimation(playAction);
		
		if (playAction.SourcePlayer.Id == Match.LocalPlayerId)
		{
			var playCardTween = PlayCardAnimation(playAction);
			while (playCardTween.IsRunning())
				yield return null;
			
			var handTweens = TweenToHandPositions();
			while (handTweens.Any(t => t.IsRunning()))
				yield return null;
			
			CalculatePreviewBoundaries();
			QueueRedraw();
		}

		while (tokenTween.IsRunning())
			yield return null;
	}

	private Tween PlayCardAnimation(PlayCardAction action)
	{
		const double tweenDuration = 0.4;
		var cardView = _cards.Single(c => c.Card == action.CardToPlay);

		var targetPosition = _mapView.GetTileGlobalPosition(action.TargetTile);
		
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		tween.TweenProperty(cardView, "position", ToLocal(targetPosition), tweenDuration);
		tween.TweenProperty(cardView, "scale", Vector2.One * 0.2f, tweenDuration).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(cardView, "modulate:a", 0f, tweenDuration);
		tween.Chain().TweenCallback(Callable.From(() =>
		{
			_cards.Remove(cardView);
			cardView.QueueFree();
		}));
		
		return tween;
	}

	private int CalculateHoveredIndex(Vector2 mousePos)
	{
		for (int i = 0; i < _cards.Count; i++)
		{
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
		tween.TweenProperty(card, "position", handPos + Vector2.Up * (60f + handPos.Y), tweenDuration);
		tween.TweenProperty(card, "rotation", 0, tweenDuration);
		tween.TweenProperty(card, "scale", Vector2.One * 1.3f, tweenDuration);
		
		_tweenTracker.TrackTween(tween, card);
		return tween;
	}

	private void CalculatePreviewBoundaries()
	{
		if (_cards.Count > 0)
		{
			_previewXMin = (int)(_cards[0].Position.X - 100);
			_previewXMax = (int)(_cards[^1].Position.X + 100);
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
		cardView.Position = (Vector2.Left * 1200) + (Vector2.Down * 400);
			
		_cards.Add(cardView);
		AddChild(cardView);
		
		cardView.Initialize(Game, card);
	}
}
