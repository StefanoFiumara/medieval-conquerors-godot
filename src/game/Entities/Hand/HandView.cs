using System.Collections.Generic;
using Godot;
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

/// <summary>
/// Handles Selection, Hover Effects, and Positioning of CardViews in the player's hand
/// </summary>
public partial class HandView : Node2D, IGameComponent
{
	public static readonly Vector2 DeckPosition = new(-1200, 400);
	public static readonly Vector2 DiscardPosition = new(1200, 400);

	private const float HAND_WIDTH = 600;
	private const float HAND_HEIGHT = 95f;
	private const int PREVIEW_SECTION_HEIGHT = 350;

	private int _hoverXMin;
	private int _hoverXMax;
	private int _hoverSectionSize = 150;

	private int _hoveredIndex = -1;
	private int _selectedIndex = -1;

	private Viewport _viewport;
	private IGameSettings _settings;
	private EventAggregator _events;

	private PackedScene _cardScene;

	public List<CardView> Cards { get; } = [];
	private readonly TweenTracker<CardView> _tweenTracker = new();

	// TODO: Set map view via Game container? maybe in _Ready?
	[Export] private MapView _mapView;
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;

	public IGame Game { get; set; }

	public override void _EnterTree()
	{
		_cardScene = ResourceLoader.Load<PackedScene>("uid://b53wqwu1youqe");
		GetParent<GameController>().Game.AddComponent(this);

		_events = Game.GetComponent<EventAggregator>();
		_settings = Game.GetComponent<IGameSettings>();

		_viewport = GetViewport();
		CenterView();
		_viewport.SizeChanged += CenterView;
	}

	public override void _ExitTree() => _viewport.SizeChanged -= CenterView;

	private void CenterView()
	{
		var visibleRect = _viewport.GetVisibleRect();
		Scale = visibleRect.CalculateScaleFactor();
		Position = new Vector2(visibleRect.Size.X * 0.5f, visibleRect.Size.Y - (165f * Scale.Y));
	}

	public override void _Process(double elapsed)
	{
		if (!Game.IsIdle()) return;

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
				 _events.Publish(InputSystem.CLICKED_EVENT, (IClickable)null, mouseEvent);
			}
			if (mouseEvent.ButtonIndex == MouseButton.Left && _hoveredIndex != -1)
			{
				_events.Publish(InputSystem.CLICKED_EVENT, Cards[_hoveredIndex], mouseEvent);
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
				height: PREVIEW_SECTION_HEIGHT
			);

			DrawRect(sectionRect, Colors.Magenta, false, 2);
		}
	}

	/* CARD SELECTION */
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

	public Tween SelectCardTween(CardView card)
	{
		const float SELECTED_SCALE = 0.7f;
		const double TWEEN_DURATION = 0.3;
		// TODO: Formalize highlight colors in one file (Game settings?)
		SetSelected(card);
		card.Highlight(Colors.Cyan);

		var tween = card.CreateTween().SetParallel().SetTrans(Tween.TransitionType.Sine);

		// TODO: Store the card's width/height as constants in the CardView class
		//		 Figure out if it's possible to derive these values from the card scene itself
		tween.TweenProperty(card, "global_position", Vector2.Right * 135 * SELECTED_SCALE + Vector2.Down * 185 * SELECTED_SCALE, TWEEN_DURATION);
		tween.TweenProperty(card, "scale", Vector2.One * SELECTED_SCALE, TWEEN_DURATION);

		return tween;
	}

	/* CARD HOVER */
	private int CalculateHoveredIndex(Vector2 mousePos)
	{
		for (int i = 0; i < Cards.Count; i++)
		{
			var sectionRect = new Rect2(
				x: _hoverXMin + i * _hoverSectionSize,
				y: -150,
				width: _hoverSectionSize,
				height: PREVIEW_SECTION_HEIGHT
			);

			if (sectionRect.HasPoint(mousePos))
				return i;
		}

		return -1;
	}

	private void UpdateHoverSections()
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

	private Tween HoverCardTween(int index)
	{
		const double TWEEN_DURATION = 0.2;

		var card = Cards[index];
		card.ZIndex = 100;

		var tween = CreateTween()
			.SetTrans(Tween.TransitionType.Sine)
			.SetParallel();

		var (handPos, _) = GetCardPosition(card);
		tween.TweenProperty(card, "position", handPos + Vector2.Up * (30 + handPos.Y), TWEEN_DURATION);
		tween.TweenProperty(card, "rotation", 0, TWEEN_DURATION);

		_tweenTracker.TrackTween(tween, card);
		return tween;
	}

	/* CARD MANAGEMENT */
	public CardView CreateCardView(Card card, Vector2 position = default)
	{
		var cardView = _cardScene.Instantiate<CardView>();
		cardView.ZAsRelative = false;
		cardView.Position = position;

		Cards.Add(cardView);
		AddChild(cardView);

		cardView.Load(Game, card);

		return cardView;
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

			var (targetPosition, targetRotation) = GetCardPosition(card);

			tween.TweenProperty(card, "position", targetPosition, duration);
			tween.TweenProperty(card, "rotation", targetRotation, duration);
			tween.TweenProperty(card, "scale", Vector2.One, duration);

			tweens.Add(tween);
			_tweenTracker.TrackTween(tween, card);
		}

		if(tweens.Count > 0)
			tweens[^1].Chain().TweenCallback(Callable.From(UpdateHoverSections));

		return tweens;
	}

	private (Vector2 position, float rotation) GetCardPosition(CardView card)
	{
		var ratio = 0.5f;
		if (Cards.Count > 1)
			ratio = Cards.IndexOf(card) / (float) (Cards.Count - 1);

		var xOffset = _spreadCurve.Sample(ratio) * HAND_WIDTH * (Cards.Count / 10f);
		var yOffset = -_heightCurve.Sample(ratio) * HAND_HEIGHT;
		var position = new Vector2(xOffset, yOffset);

		var rotation = -_rotationCurve.Sample(ratio) * 0.25f * (Cards.Count / 5f);

		return (position, rotation);
	}
}
