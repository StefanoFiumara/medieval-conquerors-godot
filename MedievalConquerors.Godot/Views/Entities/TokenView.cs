using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Views.Entities;

public partial class TokenView : Node2D, IClickable
{
	[Export] private Sprite2D _image;
	public Card Card { get; private set; }

	public void Initialize(IGame game, Card card)
	{
		Card = card;
		if (!string.IsNullOrEmpty(Card.CardData.ImagePath))
		{
			_image.Texture = GD.Load<Texture2D>(Card.CardData.ImagePath);
		}
	}
}
