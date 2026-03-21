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

		// TODO: Update CardData with token frame ID instead of UID
		//		 Set token frame ID in _icon sprite
		//		Set up Garrison View
		// if (ResourceUid.TextToId(card.Data.TokenFrameId) != ResourceUid.InvalidId)
		// 	_badge.Texture = GD.Load<Texture2D>(Card.Data.TokenFrameId);

		// NOTE: The Match Player ID matches up to the frame we want to display for the badge:
		//		0 == Local Player
		//		1 == Enemy Player
		_badge.Frame = card.Owner.Id;

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
