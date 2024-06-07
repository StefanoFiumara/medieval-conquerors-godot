using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Views.Entities;

public partial class CardView : Node2D, IClickable
{
	[Export] private Label _title;
	[Export] private RichTextLabel _description;
	[Export] private Sprite2D _image;
	[Export] private NinePatchRect _glow;
	
	[Export] private Label _foodCost;
	[Export] private Label _woodCost;
	[Export] private Label _goldCost;
	[Export] private Label _stoneCost;
	
	[Export] private TextureRect _foodIcon;
	[Export] private TextureRect _woodIcon;
	[Export] private TextureRect _goldIcon;
	[Export] private TextureRect _stoneIcon;

	private CardSystem _cardSystem;

	private Tween _glowTween;
	
	private Color _targetHighlightColor;
	private Color TargetHighlightColor
	{
		get => _targetHighlightColor;
		set
		{
			if (_targetHighlightColor == value) return;
			
			_targetHighlightColor = value;
			if (value == Colors.Transparent)
			{
				_glowTween?.Kill();
				_glowTween = CreateTween().SetEase(Tween.EaseType.InOut);
				_glowTween.TweenProperty(_glow, "modulate", Colors.Transparent, 0.25f);
			}
			else
			{
				_glowTween = CreateTween().SetLoops().SetEase(Tween.EaseType.InOut);
				_glowTween.TweenProperty(_glow, "modulate", _targetHighlightColor, 0.5f);
				var highlightGlow = _targetHighlightColor with { A = 0.7f };
				_glowTween.TweenProperty(_glow, "modulate", highlightGlow, 0.5f);
			}
		}
	}
	
	public Card Card { get; private set; }

	public void Initialize(IGame game, Card card)
	{
		Card = card;
		_cardSystem = game.GetComponent<CardSystem>();
		
		// TODO: Update title color based on card type?
		_title.Text = Card.CardData.Title;
		
		// TODO: Append tags and card type to description
		_description.Text = Card.CardData.Description;
		RemoveHighlight();
		
		// NOTE: Assumes _cardData.Image is a 256x256 sprite.
		if (!string.IsNullOrEmpty(Card.CardData.ImagePath))
		{
			_image.Texture = GD.Load<Texture2D>(Card.CardData.ImagePath);
		}
		
		var cost = Card.Attributes.OfType<ResourceCostAttribute>().SingleOrDefault();
		if (cost != null)
		{
			_foodCost.Text = $"{cost.Food}";
			_woodCost.Text = $"{cost.Wood}";
			_goldCost.Text = $"{cost.Gold}";
			_stoneCost.Text = $"{cost.Stone}";
			
			_foodIcon.Visible = cost.Food > 0;
			_woodIcon.Visible = cost.Wood > 0;
			_goldIcon.Visible = cost.Gold > 0;
			_stoneIcon.Visible = cost.Stone > 0;
			
			_foodCost.Visible = cost.Food > 0;
			_woodCost.Visible = cost.Wood > 0;
			_goldCost.Visible = cost.Gold > 0;
			_stoneCost.Visible = cost.Stone > 0;
		}
		else
		{
			// TODO: Hide Resource Panel? Is this ever the case?
		}
	}

	public void Highlight()
	{
		var color = _cardSystem.IsPlayable(Card) ? Colors.LawnGreen : Colors.IndianRed;
		TargetHighlightColor = color;
	}

	public void Highlight(Color color)
	{
		TargetHighlightColor = color;
	}
	
	public void RemoveHighlight()
	{
		TargetHighlightColor = Colors.Transparent;
	}
}
