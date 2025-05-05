using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Entities.Tokens;

public partial class TokenView : Node2D, IClickable
{
	[Export] private Sprite2D _image;

	public Card Card { get; private set; }

	public void Initialize(IGame game, Card card)
	{
		Card = card;

		if (ResourceUid.TextToId(card.CardData.TokenImagePath) != ResourceUid.InvalidId)
			_image.Texture = GD.Load<Texture2D>(Card.CardData.TokenImagePath);

		// NOTE: The Match Player ID matches up to the frame we want to display for this token:
		//		0 == Local Player
		//		1 == Enemy Player
		_image.Frame = card.Owner.Id;

		// TODO: Set up glow tile based on alliance?
		// TODO: Disable glow tile entirely and just focus on tooltip?

		// TODO: Put this info in a tooltip
		UpdateGarrisonInfo();
	}

	public void UpdateGarrisonInfo()
	{
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
