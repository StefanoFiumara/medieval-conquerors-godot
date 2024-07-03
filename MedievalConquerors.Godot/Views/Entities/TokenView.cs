using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Input;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Views.Entities;

public partial class TokenView : Node2D, IClickable
{
	[Export] private Sprite2D _image;
	[Export] private Label _garrisonInfo;
	
	public Card Card { get; private set; }

	public void Initialize(IGame game, Card card)
	{
		Card = card;
		if (!string.IsNullOrEmpty(Card.CardData.ImagePath))
		{
			_image.Texture = GD.Load<Texture2D>(Card.CardData.ImagePath);
		}

		UpdateGarrisonInfo();
	}

	public void UpdateGarrisonInfo()
	{
		var garrison = Card.GetAttribute<GarrisonCapacityAttribute>();
		if (garrison == null)
		{
			_garrisonInfo.Hide();
		}
		else
		{
			_garrisonInfo.Text = $"{garrison.Units.Count}/{garrison.Limit}";
		}
	}
}
