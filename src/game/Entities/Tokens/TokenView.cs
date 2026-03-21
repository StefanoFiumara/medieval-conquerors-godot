using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Entities.Tokens;

public partial class TokenView : Node2D, IClickable
{
	[Export] private Sprite2D _badge;
	[Export] private Sprite2D _icon;

	public Card Card { get; private set; }

	public void Initialize(IGame game, Card card)
	{
		Card = card;

		// NOTE: The Match Player ID matches up to the frame we want to display for the badge:
		//		0 == Local Player
		//		1 == Enemy Player
		_badge.Frame = card.Owner.Id;
		_icon.Frame = card.Data.TokenFrameId;

		// TODO: Set up Garrison View
		// IDEA: Calculate builder token position in code?
		SetGarrisonView(0);
	}

	public void SetGarrisonView(int garrisonCount)
	{
		// TODO: set up radial view around the badge with garrisoned villager icons
		var garrison = Card.GetAttribute<GarrisonCapacityAttribute>();
		if (garrison == null)
		{
			//_garrisonInfo.Hide();
		}
		else
		{
			//_garrisonInfo.Text = $"{garrison.Units.Count}/{garrison.Limit}";
		}
	}
}
