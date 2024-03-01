using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Views.Entities;

public partial class HandView : Node2D
{
	// TODO: Difference HandWidth based on card count? experiment with this
	private const float MaxHandWidth = 750f;
	private const float HandHeight = 95f;
	
	private readonly List<CardView> _cards = new();
	private PackedScene _cardScene;
	
	[Export] private Curve _spreadCurve;
	[Export] private Curve _heightCurve;
	[Export] private Curve _rotationCurve;
	
	public override void _Ready()
	{
		_cardScene = GD.Load<PackedScene>("res://Views/Entities/Card.tscn");
		
		// spawning some test cards
		for (int i = 0; i < 10; i++)
		{
			AddCard();
		}
		
		// set card positions
		SetCardPositions();
	}

	private (Vector2 pos, float rot) CalculateHandPosition(CardView card)
	{
		var ratio = CalculateRatio(card);
		var xOffset = _spreadCurve.Sample(ratio) * (MaxHandWidth * (_cards.Count / 12f));
		var yOffset = -_heightCurve.Sample(ratio) * (HandHeight * (_cards.Count / 10f));
		var rotation = -_rotationCurve.Sample(ratio) * (0.25f * (_cards.Count / 10f));

		var pos = new Vector2(xOffset, yOffset);
		var rot = rotation;

		return (pos, rot);
	}

	private void SetCardPositions()
	{
		var tween = CreateTween()
				.SetTrans(Tween.TransitionType.Sine)
				.SetParallel();
		;
		foreach (var card in _cards)
		{
			var (pos, rot) = CalculateHandPosition(card);
			
			tween.TweenProperty(card, "position", pos, 0.3);
			tween.TweenProperty(card, "rotation", rot, 0.3);
			//tween.Chain();

			// card.Position = pos;
			// card.Rotation = rot;
		}
	}

	// TEMP: Testing different hand counts
	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsEcho()) return;
		
		if (inputEvent is InputEventKey e && e.IsPressed())
		{
			if (e.Keycode == Key.Left)
			{
				RemoveCard();
				SetCardPositions();
			}
			else if (e.Keycode == Key.Right)
			{
				AddCard();
				SetCardPositions();
			}
		}
	}
	
	public void AddCard()
	{
		// TEMP: just for testing, limit hand card count to 10
		if (_cards.Count < 10)
		{
			var cardView = _cardScene.Instantiate<CardView>();
			// TODO: Initialize CardView with CardData
			// instance.Initialize(card);
			
			// Spawn card offscreen
			// TODO: formalize this? or is this ok?
			cardView.Position = (Vector2.Left * 1200) + (Vector2.Down * 300);
			
			_cards.Add(cardView);
			AddChild(cardView);
		}
	}

	public void RemoveCard()
	{
		if (_cards.Count > 1)
		{
			var card = _cards[0];
			_cards.Remove(card);
			card.QueueFree();
		}
	}
	
	private float CalculateRatio(CardView card)
	{
		var ratio = 0.5f;
		if (_cards.Count > 1)
			ratio = _cards.IndexOf(card) / (float) (_cards.Count - 1);

		return ratio;
	}
}
