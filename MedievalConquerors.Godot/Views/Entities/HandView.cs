using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Views.Entities;

public partial class HandView : Node2D
{
	private const float HandWidth = 450f;
	private const float HandHeight = 75f;
	
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
			var instance = _cardScene.Instantiate<CardView>();
			_cards.Add(instance);
			AddChild(instance);
		}
		
		// set card positions
		foreach (var card in _cards)
		{
			var ratio = CalculateRatio(card);
			var xOffset = _spreadCurve.Sample(ratio) * HandWidth;
			var yOffset = -_heightCurve.Sample(ratio) * HandHeight;
			var rotation = -_rotationCurve.Sample(ratio) * 0.3f;

			card.Position = new Vector2(xOffset, yOffset);
			card.Rotation = rotation;
		}
	}

	private float CalculateRatio(Node card)
	{
		var ratio = 0.5f;
		if (_cards.Count > 1)
			ratio = card.GetIndex() / (float) (_cards.Count - 1);

		return ratio;
	}
}
