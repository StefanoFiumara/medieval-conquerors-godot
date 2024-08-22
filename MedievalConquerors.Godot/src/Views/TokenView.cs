using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Views;

public partial class TokenView : Node2D, IClickable
{
	[Export] private Sprite2D _image;
	
	public Card Card { get; private set; }

	public void Initialize(IGame game, Card card)
	{
		Card = card;
		
		if (!string.IsNullOrEmpty(Card.CardData.TokenImagePath)) 
			_image.Texture = GD.Load<Texture2D>(Card.CardData.TokenImagePath);

		// NOTE: The Match Player ID matches up to the frame we want to display for this token:
		//		0 == Local Player
		//		1 == Enemy Player
		_image.Frame = card.Owner.Id;
		
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