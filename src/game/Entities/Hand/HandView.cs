using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
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

	private int _hoverXMin;
	private int _hoverXMax;
	private int _hoverSectionWidth;

	private int _hoveredIndex = -1;
	private int _selectedIndex = -1;

	// TODO: We should not be caching viewport, this can have issues in multi-scene setups (causing the reference to go stale)
	private Viewport _viewport;
	private CardSystem _cardSystem;
	private TargetSystem _targetSystem;

	public List<CardView> Cards { get; } = [];
	private readonly Dictionary<Area2D, CardView> _areaMap = [];
	private readonly TweenTracker<CardView> _tweenTracker = new();

	[Export] private MapView _mapView;
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;

	private TargetingIndicator _targetIndicator;

	public IGame Game { get; set; }

	// TODO: can this be _Ready instead of EnterTree?
	public override void _EnterTree()
	{
		GetParent<GameController>().Game.AddComponent(this);
		_cardSystem = Game.GetComponent<CardSystem>();
		_targetSystem = Game.GetComponent<TargetSystem>();
		_targetIndicator = GetNode<TargetingIndicator>("%target_indicator");
		_viewport = GetViewport();
		_viewport.Connect(Viewport.SignalName.SizeChanged, Callable.From(CenterView));
		CenterView();
	}

	private void CenterView()
	{
		var visibleRect = _viewport.GetVisibleRect();
		Scale = visibleRect.CalculateScaleFactor();
		Position = new Vector2(visibleRect.Size.X * 0.5f, visibleRect.Size.Y - (165f * Scale.Y));
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!DisplayServer.WindowIsFocused())
			return;

		var mousePos = _viewport.GetMousePosition();

		if (!_viewport.GetVisibleRect().HasPoint(mousePos))
			return;

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
		var mousePos = _viewport.GetMousePosition();
		if (_selectedIndex != -1)
		{
			var card  = Cards[_selectedIndex];
			// TODO: Dragging vs targeting should take into account whether card needs target tile or not (not yet implemented)
			// When we implement this, we can just switch to targeting mode right away depending on the target requirement
			var targeting = _mapView.HoveredTile != HexMap.None;
			_targetIndicator.IsTargeting = targeting;

			var dragPosition = Vector2.Zero; //targeting ? Vector2.Zero : ToLocal(mousePos);
			card.Position = card.Position.Lerp(dragPosition, (float)elapsed * DRAG_SPEED);
			card.Scale = card.Scale.Lerp(Vector2.One, (float)elapsed * DRAG_SPEED);

			var viewportRect = _viewport.GetVisibleRect();
			if (mousePos.Y >= viewportRect.Position.Y + viewportRect.Size.Y)
				ResetSelection();
		}
	}

	public override void _UnhandledInput(InputEvent input)
	{
		// TODO: Should we set input as handled, just like in Map View?
		if (input.IsActionPressed(LEFT_CLICK) && _hoveredIndex != -1)
		{
			if(_cardSystem.IsPlayable(Cards[_hoveredIndex].Card))
			{
				_selectedIndex = _hoveredIndex;
				var targetCandidates = _targetSystem.GetTargetCandidates(Cards[_selectedIndex].Card);
				_mapView.HighlightTiles(targetCandidates, MapLayerType.SelectionHint);
				return;
			}
		}

		// TODO: maintain selection state if release comes right after selected (debounce?)
		if (input.IsActionReleased(LEFT_CLICK) && _selectedIndex != -1)
		{
			var targetCandidates = _targetSystem.GetTargetCandidates(Cards[_selectedIndex].Card);
			if (targetCandidates.Contains(_mapView.HoveredTile))
			{
				Game.Perform(new PlayCardAction(Cards[_selectedIndex].Card, _mapView.HoveredTile));
			}
			ResetSelection();
		}

		if (input.IsActionPressed(RIGHT_CLICK))
			ResetSelection();
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

	public CardView CreateCardView(Card card, Vector2 position = default)
	{
		var cardView = _cardScene.Instantiate<CardView>();
		cardView.ZAsRelative = false;
		cardView.Position = position;

		Cards.Add(cardView);
		cardView.ZIndex = Cards.Count + 10;
		AddChild(cardView);

		cardView.Load(Game, card);
		_areaMap.Add(cardView.HoverArea, cardView);
		cardView.Connect(Node.SignalName.TreeExiting, Callable.From(() =>
		{
			_areaMap.Remove(cardView.HoverArea);
			Cards.Remove(cardView);
		}));

		return cardView;
	}

	public Tween ArrangeHandTween(int? max = null)
	{
		const double TWEEN_DURATION = 0.3;

		var limit = Mathf.Min(max ?? Cards.Count, Cards.Count);

		// TODO: Should we return a "completed" tween instead so that callers don't have to null check?
		if(limit  == 0) return null;
		var sequence = CreateTween().SetParallel();

		for (var i = 0; i < limit; i++)
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

			// TODO: move isHovered logic over to GetCardPosition?
			var (targetPosition, targetRotation) = GetCardPosition(card);

			// TODO: Clean up this mess lol
			var hoverOffset = 75;
			var xOffset =
				i < _hoveredIndex
					? -hoverOffset
					: i > _hoveredIndex
						? hoverOffset
						: 0;

			if(_hoveredIndex == -1) xOffset = 0;

			var duration = isHovered ? 0.25f * TWEEN_DURATION : TWEEN_DURATION;
			if (isHovered) targetPosition.Y = -20;

			tween.TweenProperty(card, "position", targetPosition + Vector2.Right * xOffset, duration);
			tween.TweenProperty(card, "rotation", isHovered ? 0f : targetRotation, duration);
			tween.TweenProperty(card, "scale", isHovered ? 1.2f * Vector2.One : Vector2.One, duration);

			_tweenTracker.TrackTween(tween, card);

			sequence.TweenSubtween(tween);
		}

		return sequence;
	}

	public (Vector2 position, float rotation) GetCardPosition(CardView card)
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
