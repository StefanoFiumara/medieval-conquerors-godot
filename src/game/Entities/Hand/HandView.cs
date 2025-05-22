using System;
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
using MedievalConquerors.Engine.Utils;
using MedievalConquerors.Entities.Cards;
using MedievalConquerors.Entities.Maps;
using MedievalConquerors.Screens;

namespace MedievalConquerors.Entities.Hand;

public partial class HandView : Node2D, IGameComponent
{
	public static readonly Vector2 DeckPosition = new(-1200, 400);
	public static readonly Vector2 DiscardPosition = new(1200, 400);

	private const float HandWidth = 600;
	private const float HandHeight = 95f;
	private const int PreviewSectionHeight = 350;

	private int _hoverXMin;
	private int _hoverXMax;
	private int _hoverSectionSize = 150;

	private int _hoveredIndex = -1;
	private int _selectedIndex = -1;

	private Viewport _viewport;
	private ActionSystem _actionSystem;
	private IGameSettings _settings;
	private EventAggregator _events;

	private PackedScene _cardScene;

	public List<CardView> Cards { get; } = [];
	private readonly TweenTracker<CardView> _tweenTracker = new();

	[Export] private MapView _mapView;
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;

	public IGame Game { get; set; }

	public override void _EnterTree()
	{
		_cardScene = ResourceLoader.Load<PackedScene>("uid://b53wqwu1youqe");
		GetParent<GameController>().Game.AddComponent(this);

		_actionSystem = Game.GetComponent<ActionSystem>();
		_events = Game.GetComponent<EventAggregator>();
		_settings = Game.GetComponent<IGameSettings>();

		_viewport = GetViewport();
		CalculateViewPosition();
		_viewport.SizeChanged += CalculateViewPosition;
	}

	public override void _Ready()
	{
		_events.Subscribe<PlayCardAction>(GameEvent.Prepare<PlayCardAction>(), OnPreparePlayCard);
		_events.Subscribe<CreateCardAction>(GameEvent.Prepare<CreateCardAction>(), OnPrepareCreateCard);
	}

	private void OnPreparePlayCard(PlayCardAction action)      => action.PerformPhase.Viewer = PlayCardAnimation;
	private void OnPrepareCreateCard(CreateCardAction action)  => action.PerformPhase.Viewer = CreateCardAnimation;

	public override void _Process(double elapsed)
	{
		if (_actionSystem.IsActive) return;
		var mousePos = ToLocal(_viewport.GetMousePosition());
		var hovered = CalculateHoveredIndex(mousePos);

		if (hovered != _hoveredIndex)
		{
			if (hovered != -1 && hovered != _selectedIndex)
			{
				HoverCardTween(hovered);
			}

			_hoveredIndex = hovered;
			ArrangeHandTween();

			for (int i = 0; i < Cards.Count; i++)
			{
				if(i == _selectedIndex)
					continue;

				if(i == _hoveredIndex)
					Cards[_hoveredIndex].Highlight();
				else
					Cards[i].RemoveHighlight();
			}
		}
	}

	public override void _UnhandledInput(InputEvent input)
	{
		if (input is InputEventMouseButton mouseEvent && mouseEvent.IsReleased())
		{
			if (mouseEvent.ButtonIndex == MouseButton.Right)
			{
				 _events.Publish(InputSystem.ClickedEvent, (IClickable)null, mouseEvent);
			}
			if (mouseEvent.ButtonIndex == MouseButton.Left && _hoveredIndex != -1)
			{
				_events.Publish(InputSystem.ClickedEvent, Cards[_hoveredIndex], mouseEvent);
				_viewport.SetInputAsHandled();
			}
		}
	}

	public override void _Draw()
	{
		// TODO: Set the HandView's z index to a high value in order to display the debug rects over the cards in hand
		if (!_settings.DebugMode)
			return;

		for (int i = 0; i < Cards.Count; i++)
		{
			var sectionRect = new Rect2(
				x: _hoverXMin + i * _hoverSectionSize,
				y: -150,
				width: _hoverSectionSize,
				height: PreviewSectionHeight
			);

			DrawRect(sectionRect, Colors.Magenta, false, 2);
		}
	}

	private void SetSelected(CardView card)
	{
		_selectedIndex = Cards.IndexOf(card);
	}

	public void ResetSelection()
	{
		_selectedIndex = -1;
		_hoveredIndex = -1;
		ArrangeHandTween();
	}

	private void CalculateViewPosition()
	{
		var visibleRect = _viewport.GetVisibleRect();
		Scale = visibleRect.CalculateScaleFactor();
		Position = new Vector2(visibleRect.Size.X * 0.5f, visibleRect.Size.Y - (165f * Scale.Y));
	}

	private int CalculateHoveredIndex(Vector2 mousePos)
	{
		for (int i = 0; i < Cards.Count; i++)
		{
			var sectionRect = new Rect2(
				x: _hoverXMin + i * _hoverSectionSize,
				y: -150,
				width: _hoverSectionSize,
				height: PreviewSectionHeight
			);

			if (sectionRect.HasPoint(mousePos))
				return i;
		}

		return -1;
	}

	// TODO: Move to PlayCardView
	private IEnumerator PlayCardAnimation(IGame game, GameAction action)
	{
		yield return true;
		var playAction = (PlayCardAction) action;

		if (playAction.CardToPlay.Owner.Id == Match.LocalPlayerId)
		{
			var playCardTween = PlayCardTween(playAction);
			playCardTween.Chain().TweenCallback(Callable.From((Action)(() => ArrangeHandTween())));
		}
	}

	// TODO: Move to CardCreationView
	private IEnumerator CreateCardAnimation(IGame game, GameAction action)
	{
		const double tweenDuration = 0.4;
		yield return true;

		var createAction = (CreateCardAction) action;

		const float spacing = 230f;
		var center = _viewport.GetVisibleRect().GetCenter();

		List<Tween> tweens = [];
		// var cardViews = createAction.CreatedCards.Select(c => CreateCardView(c)).ToList();
		// var displayTween = DisplayCardsTween(cardViews);

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		for (var i = 0; i < createAction.CreatedCards.Count; i++)
		{
			var createdCard = createAction.CreatedCards[i];
			var cardView = CreateCardView(createdCard, Vector2.Zero);
			var offset = i - (createAction.CreatedCards.Count - 1) / 2.0f;
			cardView.GlobalPosition = center + Vector2.Right * offset * spacing;
			cardView.Scale = Vector2.Zero;
			tween.Chain().TweenInterval(0.25);
			tween.TweenProperty(cardView, "modulate:a", 1, tweenDuration).From((Variant.From(0)));
			tween.TweenProperty(cardView, "scale", Vector2.One, tweenDuration);

			if (createAction.TargetZone != Zone.Hand)
			{
				var subTween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
				var targetPosition = createAction.TargetZone == Zone.Deck
					? DeckPosition
					: DiscardPosition;

				var targetRotation = createAction.TargetZone == Zone.Deck
					? Mathf.Pi / 4
					: -Mathf.Pi / 4;

				subTween.TweenInterval(tweenDuration * i);
				subTween.Chain().TweenProperty(cardView, "position", targetPosition, tweenDuration);
				subTween.TweenProperty(cardView, "rotation", targetRotation, tweenDuration);
				subTween.Chain().TweenCallback(Callable.From(() =>
				{
					Cards.Remove(cardView);
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
			var handTweens = ArrangeHandTween();
			while (handTweens.Any(t => t.IsRunning())) yield return null;

			// CalculatePreviewBoundaries();
		}
	}

	private Tween PlayOnTileTween(CardView card, Vector2I tile, double duration = 0.5)
	{
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		var targetPosition = _mapView.GetTileGlobalPosition(tile);
		tween.TweenProperty(card, "position", ToLocal(targetPosition), duration);
		tween.TweenProperty(card, "scale", Vector2.One * 0.2f, duration).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(card, "modulate:a", 0f, duration);

		tween.Chain().TweenCallback(Callable.From(() =>
		{
			Cards.Remove(card);
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
			Cards.Remove(card);
			card.QueueFree();
		}));

		return tween;
	}

	private Tween PlayCardTween(PlayCardAction action)
	{
		// TODO: Pass cardView as parameter instead of PlayCardAction
		var cardView = Cards.SingleOrDefault(c => c.Card == action.CardToPlay);

		if (cardView == null)
		{
			var nullTween = CreateTween();
			nullTween.TweenCallback(Callable.From(() => { }));
			return nullTween;
		}

		return action.CardToPlay.CardData.CardType == CardType.Technology
			? CenterAndFadeAwayTween(cardView)
			: PlayOnTileTween(cardView, action.TargetTile);
	}

	// TODO: Use this tween in CreateCardAnimation
	public Tween DisplayCardTween(CardView card, double duration = 0.4) => DisplayCardsTween([card], duration);
	public Tween DisplayCardsTween(List<CardView> cards, double duration = 0.4)
	{
		const float spacing = 230f;

		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		for (var i = 0; i < cards.Count; i++)
		{
			var card = cards[i];
			var offset = i - (cards.Count - 1) / 2.0f;
			tween.TweenCallback(Callable.From(() =>
			{
				var center = _viewport.GetVisibleRect().GetCenter();

				card.GlobalPosition = center + Vector2.Right * offset * spacing;
				card.Scale = Vector2.Zero;
			}));

			tween.Chain().TweenInterval(duration * 0.5);
			tween.TweenProperty(card, "modulate:a", 1, duration).From((Variant.From(0)));
			tween.TweenProperty(card, "scale", Vector2.One, duration);
		}

		return tween;
	}

	public Tween SelectCardTween(CardView card)
	{
		const float selectedScale = 0.7f;
		const double tweenDuration = 0.3;
		// TODO: Formalize highlight colors in one file (Game settings?)
		SetSelected(card);
		card.Highlight(Colors.Cyan);

		var tween = card.CreateTween().SetParallel().SetTrans(Tween.TransitionType.Sine);

		// TODO: Store the card's width/height as constants in the CardView class
		//		 Figure out if it's possible to derive these values from the card scene itself
		tween.TweenProperty(card, "global_position", Vector2.Right * 135 * selectedScale + Vector2.Down * 185 * selectedScale, tweenDuration);
		tween.TweenProperty(card, "scale", Vector2.One * selectedScale, tweenDuration);

		return tween;
	}

	private Tween HoverCardTween(int index)
	{
		const double tweenDuration = 0.2;

		var card = Cards[index];
		card.ZIndex = 100;

		var tween = CreateTween()
			.SetTrans(Tween.TransitionType.Sine)
			.SetParallel();

		var (handPos, _) = CalculateHandPosition(card);
		tween.TweenProperty(card, "position", handPos + Vector2.Up * (30 + handPos.Y), tweenDuration);
		tween.TweenProperty(card, "rotation", 0, tweenDuration);

		_tweenTracker.TrackTween(tween, card);
		return tween;
	}

	private void CalculateHoverSections()
	{
		if (Cards.Count > 0)
		{
			// TODO: is the 100 related to the card's width? formalize this constant
			_hoverXMin = (int)(Cards[0].Position.X - 100);
			_hoverXMax = (int)(Cards[^1].Position.X + 100);
			_hoverSectionSize = (_hoverXMax - _hoverXMin) / Cards.Count;
		}
		else
		{
			_hoverXMin = 0;
			_hoverXMax = 0;
			_hoverSectionSize = 0;
		}

		QueueRedraw();
	}

	// TODO: Could it be possible to return only one tween here?
	public List<Tween> ArrangeHandTween(double duration = 0.25)
	{
		var tweens = new List<Tween>();

		for (var i = 0; i < Cards.Count; i++)
		{
			// Do not animate hovered/selected card.
			if (i == _hoveredIndex)  continue;
			if (i == _selectedIndex) continue;

			var card = Cards[i];
			card.ZIndex = i + 10;

			var tween = CreateTween()
				.SetTrans(Tween.TransitionType.Sine)
				.SetParallel();

			var (targetPosition, targetRotation) = CalculateHandPosition(card);

			tween.TweenProperty(card, "position", targetPosition, duration);
			tween.TweenProperty(card, "rotation", targetRotation, duration);
			tween.TweenProperty(card, "scale", Vector2.One, duration);

			tweens.Add(tween);
			_tweenTracker.TrackTween(tween, card);
		}

		if(tweens.Count > 0)
			tweens[^1].Chain().TweenCallback(Callable.From(CalculateHoverSections));

		return tweens;
	}

	private (Vector2 position, float rotation) CalculateHandPosition(CardView card)
	{
		var ratio = 0.5f;
		if (Cards.Count > 1)
			ratio = Cards.IndexOf(card) / (float) (Cards.Count - 1);

		var xOffset = _spreadCurve.Sample(ratio) * HandWidth * (Cards.Count / 10f);
		var yOffset = -_heightCurve.Sample(ratio) * HandHeight;
		var position = new Vector2(xOffset, yOffset);

		var rotation = -_rotationCurve.Sample(ratio) * 0.25f * (Cards.Count / 5f);

		return (position, rotation);
	}

	public CardView CreateCardView(Card card, Vector2 position = default)
	{
		var cardView = _cardScene.Instantiate<CardView>();
		cardView.Position = position;

		Cards.Add(cardView);
		AddChild(cardView);

		cardView.Load(Game, card);

		return cardView;
	}

	public override void _ExitTree() => _viewport.SizeChanged -= CalculateViewPosition;
}
