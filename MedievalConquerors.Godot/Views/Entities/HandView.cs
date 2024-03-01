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

	private void SetCardPositions()
	{
		foreach (var card in _cards)
		{
			var ratio = CalculateRatio(card);
			var xOffset = _spreadCurve.Sample(ratio) * (MaxHandWidth * (_cards.Count / 12f));
			var yOffset = -_heightCurve.Sample(ratio) * (HandHeight * (_cards.Count / 10f));
			var rotation = -_rotationCurve.Sample(ratio) * (0.25f * (_cards.Count / 10f));

			card.Position = new Vector2(xOffset, yOffset);
			card.Rotation = rotation;
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
		if (_cards.Count < 10)
		{
			var instance = _cardScene.Instantiate<CardView>();
			_cards.Add(instance);
			AddChild(instance);
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
