using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Engine.GameComponents;
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
	private static readonly PackedScene _cardScene = GD.Load<PackedScene>("uid://b53wqwu1youqe");
	public static readonly Vector2 DeckPosition = new(-1200, 400);
	public static readonly Vector2 DiscardPosition = new(1200, 400);

	// TODO: maybe formalize into select/cancel actions
	private const string LEFT_CLICK = "left_click";
	private const string RIGHT_CLICK = "right_click";

	private const float DRAG_SPEED = 20f;
	private const float HAND_WIDTH = 700;
	private const float HAND_HEIGHT = 250;

	public IGame Game { get; set; }
	public List<CardView> Cards { get; } = [];

	private int _hoverXMin;
	private int _hoverXMax;
	private int _hoverSectionWidth;

	private int _hoveredIndex = -1;
	private int _selectedIndex = -1;

	private CardSystem _cardSystem;
	private TargetSystem _targetSystem;
	private TargetingIndicator _targetIndicator;

	private readonly Dictionary<Area2D, CardView> _areaMap = [];
	private readonly TweenTracker<CardView> _tweenTracker = new();

	[Export] private MapView _mapView;
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;

	// TODO: can this be _Ready instead of EnterTree?
	public override void _EnterTree()
	{
		GetParent<GameController>().Game.AddComponent(this);
		_cardSystem = Game.GetComponent<CardSystem>();
		_targetSystem = Game.GetComponent<TargetSystem>();
		_targetIndicator = GetNode<TargetingIndicator>("%target_indicator");
		GetViewport().Connect(Viewport.SignalName.SizeChanged, Callable.From(CenterView));
		CenterView();
	}

	private void CenterView()
	{
		var visibleRect = GetViewport().GetVisibleRect();
		Scale = visibleRect.CalculateScaleFactor();
		Position = new Vector2(visibleRect.Size.X * 0.5f, visibleRect.Size.Y - (165f * Scale.Y));
	}

	public override void _PhysicsProcess(double delta)
	{
		var viewport = GetViewport();
		var mousePos = viewport.GetMousePosition();

		// TODO: If cards get removed from the Cards list as soon as they begin animating (rather than when they are freed)
		//		Then we will probably experience less jitters without having to turn off hover effects when the engine is active.
		// if (!Game.IsIdle()) return;
		if (!DisplayServer.WindowIsFocused()) return;
		if (!viewport.GetVisibleRect().HasPoint(mousePos)) return;
		if (_selectedIndex != -1) return;

		var hovered = CheckHoveredIndex(mousePos);
		if (hovered != _hoveredIndex)
		{
			_hoveredIndex = hovered;
			if (_hoveredIndex != -1)
			{
				var cardView = Cards[_hoveredIndex];
				cardView.Highlight();
				cardView.ZIndex = 100;
			}

			for (int j = 0; j < Cards.Count; j++)
			{
				if (j == _hoveredIndex) continue;
				Cards[j].RemoveHighlight();
			}

			ArrangeHandTween();
		}
	}

	private int CheckHoveredIndex(Vector2 mousePos)
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var param = new PhysicsPointQueryParameters2D
		{
			Position = mousePos,
			CollideWithAreas = true,
			CollideWithBodies = false
		};

		var result = spaceState.IntersectPoint(param);
		foreach (var collided in result)
		{
			if (!collided.TryGetValue("collider", out var collider)) continue;

			var area = (Area2D)collider;
			if (_areaMap.TryGetValue(area, out var cardView))
				return Cards.IndexOf(cardView);
		}

		return -1;
	}

	public override void _Process(double elapsed)
	{
		var viewport = GetViewport();
		var mousePos = viewport.GetMousePosition();
		if (_selectedIndex != -1)
		{
			var card  = Cards[_selectedIndex];
			// TODO: Dragging vs targeting should take into account whether card needs target tile or not (not yet implemented)
			// When we implement this, we can just switch to targeting mode right away depending on the target requirement
			_targetIndicator.IsTargeting = _mapView.HoveredTile != HexMap.None;

			var dragPosition = Vector2.Zero;
			card.Position = card.Position.Lerp(dragPosition, (float)elapsed * DRAG_SPEED);
			card.Scale = card.Scale.Lerp(Vector2.One, (float)elapsed * DRAG_SPEED);

			var viewportRect = viewport.GetVisibleRect();
			if (mousePos.Y >= viewportRect.Position.Y + viewportRect.Size.Y)
				ResetSelection();
		}
	}

	public override void _UnhandledInput(InputEvent input)
	{
		if (input.IsActionPressed(LEFT_CLICK) && _hoveredIndex != -1)
		{
			if(_cardSystem.IsPlayable(Cards[_hoveredIndex].Card))
			{
				_selectedIndex = _hoveredIndex;
				var targetCandidates = _targetSystem.GetTargetCandidates(Cards[_selectedIndex].Card);
				_mapView.HighlightTiles(targetCandidates, MapLayerType.SelectionHint);
				GetViewport().SetInputAsHandled();
				return;
			}
		}

		// TODO: maintain selection state if release comes right after selected (debounce?)
		if (input.IsActionReleased(LEFT_CLICK) && _selectedIndex != -1)
		{
			var targetCandidates = _targetSystem.GetTargetCandidates(Cards[_selectedIndex].Card);
			if (targetCandidates.Contains(_mapView.HoveredTile))
			{
				// TODO: If ActionSystem is active, add this action to a queue instead, to be performed once the current sequence is finished.
				Game.Perform(new PlayCardAction(Cards[_selectedIndex].Card, _mapView.HoveredTile));
			}

			ResetSelection();
			GetViewport().SetInputAsHandled();
		}

		if (input.IsActionPressed(RIGHT_CLICK))
		{
			ResetSelection();
			GetViewport().SetInputAsHandled();
		}
	}

	private void ResetSelection()
	{
		if (_selectedIndex == -1) return;

		Cards[_selectedIndex].RemoveHighlight();
		_selectedIndex = -1;
		_hoveredIndex = -1;
		_mapView.ResetSelection(MapLayerType.SelectionHint);
		_targetIndicator.IsTargeting = false;
		ArrangeHandTween();
	}

	public CardView AddCardView(CardView existing)
	{
		existing.ZAsRelative = false;
		Cards.Add(existing);
		existing.ZIndex = Cards.Count + 10;
		_areaMap.Add(existing.HoverArea, existing);

		existing.Connect(Node.SignalName.TreeExiting, Callable.From(() =>
		{
			_areaMap.Remove(existing.HoverArea);
			if(_selectedIndex == Cards.IndexOf(existing))
				ResetSelection();

			Cards.Remove(existing);
		}));

		return existing;
	}

	public CardView CreateCardView(Card card, Vector2 position = default)
	{
		var cardView = _cardScene.Instantiate<CardView>();
		cardView.ZAsRelative = false;
		cardView.Position = position;
		AddChild(cardView);
		cardView.Load(Game, card);
		return cardView;
	}

	public Tween ArrangeHandTween(int? totalCount = null)
	{
		const double TWEEN_DURATION = 0.3;

		if(Cards.Count == 0)
			return this.NullTween;

		var sequence = CreateTween().SetParallel();

		for (var i = 0; i < Cards.Count; i++)
		{
			// Do not animate selected
			if (i == _selectedIndex) continue;

			var isHovered = i == _hoveredIndex;
			var card = Cards[i];
			card.ZIndex = isHovered ? 100 : i + 10;

			var tween = CreateTween()
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.Out)
				.SetParallel();

			var (targetPosition, targetRotation, targetScale) = CalculateViewPosition(index: i, totalCount ?? Cards.Count);

			// TODO: if card is already in position - skip the tweener

			var duration = isHovered ? 0.25f * TWEEN_DURATION : TWEEN_DURATION;

			tween.TweenProperty(card, "position", targetPosition, duration);
			tween.TweenProperty(card, "rotation",  targetRotation, duration);
			tween.TweenProperty(card, "scale", targetScale, duration);

			_tweenTracker.TrackTween(tween, card);

			sequence.TweenSubtween(tween);
		}

		return sequence;
	}

	public (Vector2 position, float rotation, Vector2 scale) CalculateViewPosition(int index, int totalCount)
	{
		const int HOVER_OFFSET_X = 85;
		const int HOVER_OFFSET_Y = 60;

		var isHovered = index == _hoveredIndex;

		// calculate a horizontal offset based on whether its to the left/right of the hovered card
		var xOffset = 0;
		if (index < _hoveredIndex && _hoveredIndex != -1)
			xOffset = -HOVER_OFFSET_X;
		else if (index > _hoveredIndex && _hoveredIndex != -1)
			xOffset = HOVER_OFFSET_X;

		// Calculate where to sample the spread/height curves based on the card's order in the hand
		var ratio = 0.5f;
		if (totalCount > 1)
			ratio = index / (float) (totalCount - 1);

		var targetX = _spreadCurve.Sample(ratio) * HAND_WIDTH * (totalCount / 10f) + xOffset;
		var targetY = isHovered ? -HOVER_OFFSET_Y : -_heightCurve.Sample(ratio) * HAND_HEIGHT;

		var position = new Vector2(targetX, targetY);
		var rotation = isHovered ? 0f : -_rotationCurve.Sample(ratio) * 0.25f * (totalCount / 5f);
		var scale = (isHovered ? 1.4f : 1f) * Vector2.One;

		return (position, rotation, scale);
	}
}
