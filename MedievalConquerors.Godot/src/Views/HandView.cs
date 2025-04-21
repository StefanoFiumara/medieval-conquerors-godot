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

namespace MedievalConquerors.Views;

public partial class HandView : Node2D, IGameComponent
{
	private const float MaxHandWidth = 850f;
	private const float HandHeight = 95f;
	private const int PreviewSectionHeight = 350;

	private int _previewXMin;
	private int _previewXMax;
	private int _previewSectionSize = 150;

	private int _hoveredIndex = -1;
	private int _selectedIndex = -1;

	private Viewport _viewport;
	private ActionSystem _actionSystem;
	private IGameSettings _settings;
	private EventAggregator _events;

	private readonly List<CardView> _cards = [];
	private readonly TweenTracker<CardView> _tweenTracker = new();

	[Export] private MapView _mapView;
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;
	[Export] private PackedScene _cardScene;

	public IGame Game { get; set; }

	public override void _Ready()
	{
		GetParent<GameController>().Game.AddComponent(this);

		_actionSystem = Game.GetComponent<ActionSystem>();
		_events = Game.GetComponent<EventAggregator>();
		_settings = Game.GetComponent<IGameSettings>();

		_events.Subscribe<PlayCardAction>(GameEvent.Prepare<PlayCardAction>(), OnPreparePlayCard);
		_events.Subscribe<DrawCardsAction>(GameEvent.Prepare<DrawCardsAction>(), OnPrepareDrawCards);
		_events.Subscribe<DiscardCardsAction>(GameEvent.Prepare<DiscardCardsAction>(), OnPrepareDiscardCards);
		_events.Subscribe<CreateCardAction>(GameEvent.Prepare<CreateCardAction>(), OnPrepareCreateCard);

		_viewport = GetViewport();
		CalculateViewPosition();
		_viewport.SizeChanged += CalculateViewPosition;
	}

	private void OnPrepareDrawCards(DrawCardsAction action)       => action.PerformPhase.Viewer = DrawCardsAnimation;
	private void OnPrepareDiscardCards(DiscardCardsAction action) => action.PerformPhase.Viewer = DiscardCardsAnimation;
	private void OnPreparePlayCard(PlayCardAction action)         => action.PerformPhase.Viewer = PlayCardAnimation;
	private void OnPrepareCreateCard(CreateCardAction action)     => action.PerformPhase.Viewer = CreateCardAnimation;

	public override void _Process(double elapsed)
	{
		if (_actionSystem.IsActive) return;
		var mousePos = ToLocal(_viewport.GetMousePosition());
		var hovered = CalculateHoveredIndex(mousePos);

		if (hovered != _hoveredIndex)
		{
			if (hovered != -1 && hovered != _selectedIndex)
			{
				TweenToPreviewPosition(hovered);
			}

			_hoveredIndex = hovered;
			TweenToHandPositions();

			for (int i = 0; i < _cards.Count; i++)
			{
				if(i == _selectedIndex)
					continue;

				if(i == _hoveredIndex)
					_cards[_hoveredIndex].Highlight();
				else
					_cards[i].RemoveHighlight();
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

	private void SetSelected(CardView card)
	{
		_selectedIndex = _cards.IndexOf(card);
	}

	public void ResetSelection()
	{
		_selectedIndex = -1;
		_hoveredIndex = -1;
		TweenToHandPositions();
	}

	private void CalculateViewPosition()
	{
		var visibleRect = _viewport.GetVisibleRect();
		Scale = visibleRect.CalculateScaleFactor();
		Position = new Vector2(visibleRect.Size.X * 0.5f, visibleRect.Size.Y - (165f * Scale.Y));
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
			while (tweens.Any(t => t.IsRunning())) yield return null;
		}

		CalculatePreviewBoundaries();
	}

	private IEnumerator DiscardCardsAnimation(IGame game, GameAction action)
	{
		const double tweenDuration = 0.25;
		var discardAction = (DiscardCardsAction) action;

		yield return true;

		discardAction.CardsToDiscard.Reverse();
		foreach (var card in discardAction.CardsToDiscard)
		{
			var cardView = _cards.SingleOrDefault(c => c.Card == card);
			if (cardView == null) continue;

			cardView.RemoveHighlight();

			var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();

			var targetPosition = (Vector2.Right * 1200) + (Vector2.Down * 400);
			tween.TweenProperty(cardView, "position", targetPosition, tweenDuration);
			tween.TweenProperty(cardView, "rotation", Mathf.Pi / 4, tweenDuration);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.3f, tweenDuration);

			while (tween.IsRunning()) yield return null;
			_cards.Remove(cardView);
			cardView.QueueFree();
		}
	}

	private IEnumerator PlayCardAnimation(IGame game, GameAction action)
	{
		yield return true;
		var playAction = (PlayCardAction) action;

		var tokenTween = playAction.CardToPlay.CardData.CardType != CardType.Technology
			? _mapView.PlaceTokenAnimation(playAction)
			:  null;

		if (playAction.CardToPlay.Owner.Id == Match.LocalPlayerId)
		{
			var playCardTween = PlayCardTween(playAction);
			while (playCardTween.IsRunning()) yield return null;

			var handTweens = TweenToHandPositions();
			while (handTweens.Any(t => t.IsRunning())) yield return null;

			CalculatePreviewBoundaries();
		}

		while (tokenTween != null && tokenTween.IsRunning())
			yield return null;
	}

	private Tween PlayCardTween(PlayCardAction action)
	{
		const double tweenDuration = 0.5;

		var cardView = _cards.SingleOrDefault(c => c.Card == action.CardToPlay);
		_cards.Remove(cardView);
		if (cardView == null)
		{
			var nullTween = CreateTween();
			nullTween.TweenCallback(Callable.From(() => { }));
			return nullTween;
		}

		var targetPosition = _mapView.GetTileGlobalPosition(action.TargetTile);
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		if (action.CardToPlay.CardData.CardType == CardType.Technology)
		{
			tween.TweenProperty(cardView, "global_position", _viewport.GetVisibleRect().GetCenter(), tweenDuration);
			tween.TweenProperty(cardView, "rotation", 0, tweenDuration);
			tween.TweenProperty(cardView, "scale", Vector2.One, tweenDuration).SetEase(Tween.EaseType.Out);
			tween.Chain().TweenInterval(0.4f);
			tween.TweenProperty(cardView, "scale", Vector2.One * 1.5f, tweenDuration);
		}
		else
		{
			tween.TweenProperty(cardView, "position", ToLocal(targetPosition), tweenDuration);
			tween.TweenProperty(cardView, "scale", Vector2.One * 0.2f, tweenDuration).SetEase(Tween.EaseType.Out);
		}

		tween.TweenProperty(cardView, "modulate:a", 0f, tweenDuration);
		tween.Chain().TweenCallback(Callable.From(() =>
		{
			cardView.QueueFree();
		}));

		return tween;
	}

	private IEnumerator CreateCardAnimation(IGame game, GameAction action)
	{
		const double tweenDuration = 0.4;
		yield return true;

		var createAction = (CreateCardAction) action;

		const float spacing = 230f;
		var center = _viewport.GetVisibleRect().GetCenter();

		List<Tween> tweens = [];
		var tween = CreateTween().SetTrans(Tween.TransitionType.Sine).SetParallel();
		for (var i = 0; i < createAction.CreatedCards.Count; i++)
		{
			var createdCard = createAction.CreatedCards[i];
			var cardView = CreateCardView(createdCard);
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
					? Vector2.Left * 1200 + Vector2.Down * 400
					: Vector2.Right * 1200 + Vector2.Down * 400;

				var targetRotation = createAction.TargetZone == Zone.Deck
					? Mathf.Pi / 4
					: -Mathf.Pi / 4;

				subTween.TweenInterval(tweenDuration * i);
				subTween.Chain().TweenProperty(cardView, "position", targetPosition, tweenDuration);
				subTween.TweenProperty(cardView, "rotation", targetRotation, tweenDuration);
				subTween.Chain().TweenCallback(Callable.From(() =>
				{
					_cards.Remove(cardView);
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
			var handTweens = TweenToHandPositions();
			while (handTweens.Any(t => t.IsRunning())) yield return null;

			CalculatePreviewBoundaries();
		}
	}

	public Tween SelectCardAnimation(CardView card)
	{
		const float previewScale = 0.7f;
		const double tweenDuration = 0.3;
		// TODO: Formalize highlight colors in one file (Game settings?)
		SetSelected(card);
		card.Highlight(Colors.Cyan);

		var tween = card.CreateTween().SetParallel().SetTrans(Tween.TransitionType.Sine);

		// TODO: Store the card's width/height as constants in the CardView class
		//		 Figure out if it's possible to derive these values from the card scene itself
		tween.TweenProperty(card, "global_position", Vector2.Right * 135 * previewScale + Vector2.Down * 185 * previewScale, tweenDuration);
		tween.TweenProperty(card, "scale", Vector2.One * previewScale, tweenDuration);

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
				return i;
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
		tween.TweenProperty(card, "position", handPos + Vector2.Up * (30 + handPos.Y), tweenDuration);
		tween.TweenProperty(card, "rotation", 0, tweenDuration);

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

		QueueRedraw();
	}

	private List<Tween> TweenToHandPositions()
	{
		const double tweenDuration = 0.25;
		var tweens = new List<Tween>();

		for (var i = 0; i < _cards.Count; i++)
		{
			// Do not animate hovered/selected card.
			if (i == _hoveredIndex)  continue;
			if (i == _selectedIndex) continue;

			var card = _cards[i];
			card.ZIndex = i + 10;

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

	private CardView CreateCardView(Card card)
	{
		var cardView = _cardScene.Instantiate<CardView>();

		// Spawn card offscreen, to be animated in by TweenToHandPositions
		// TODO: extract these positions into deck/discard positions
		cardView.Position = (Vector2.Left * 1200) + (Vector2.Down * 400);

		_cards.Add(cardView);
		AddChild(cardView);

		cardView.Initialize(Game, card);

		return cardView;
	}

	public override void _ExitTree() => _viewport.SizeChanged -= CalculateViewPosition;
}
