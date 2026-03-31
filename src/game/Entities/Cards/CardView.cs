using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Entities.Cards;

public partial class CardView : Node2D, IClickable
{
	// TODO: Reference via unique node names in _Ready instead of using export variables
	[Export] private RichTextLabel _title;
	[Export] private RichTextLabel _description;
	[Export] private Sprite2D _banner;
	[Export] private Label _type;
	[Export] private TextureRect _portrait;
	[Export] private NinePatchRect _glow;

	[Export] private Label _foodCost;
	[Export] private Label _woodCost;
	[Export] private Label _goldCost;
	[Export] private Label _stoneCost;

	[Export] private CenterContainer _foodIcon;
	[Export] private CenterContainer _woodIcon;
	[Export] private CenterContainer _goldIcon;
	[Export] private CenterContainer _stoneIcon;

	[Export] public Area2D HoverArea { get; private set; }
	public Card Card { get; private set; }

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
				_glowTween?.Kill();
				_glowTween = CreateTween().SetLoops().SetEase(Tween.EaseType.InOut);
				_glowTween.TweenProperty(_glow, "modulate", _targetHighlightColor, 0.5f);
				var highlightGlow = _targetHighlightColor with { A = 0.7f };
				_glowTween.TweenProperty(_glow, "modulate", highlightGlow, 0.5f);
			}
		}
	}

	public void Load(IGame game, Card card)
	{
		Card = card;
		_cardSystem = game.GetComponent<CardSystem>();

		_title.Text = $"[wave amp=90.0 freq=0 connected=0]{Card.Data.Title}[/wave]";
		_type.Text = Card.Data.CardType.ToString();
		_banner.Frame = Card.Data.CardType switch
		{
			CardType.Unit => 0,
			CardType.Building => 1,
			CardType.Technology => 2,
			CardType.Action => 3,
			_ => 0
		};

		_description.ParseBbcode(Card.Data.Description);
		RemoveHighlight();

		if (ResourceUid.TextToId(card.Data.CardPortraitUid) != ResourceUid.InvalidId)
			_portrait.Texture = GD.Load<Texture2D>(Card.Data.CardPortraitUid);

		if (Card.HasAttribute<ResourceCostAttribute>(out var cost))
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
			_foodIcon.Visible =  false;
			_woodIcon.Visible =  false;
			_goldIcon.Visible =  false;
			_stoneIcon.Visible = false;

			_foodCost.Visible =  false;
			_woodCost.Visible =  false;
			_goldCost.Visible =  false;
			_stoneCost.Visible = false;
		}
	}

	public void Highlight()
	{
		var color = _cardSystem.IsPlayable(Card) ? Colors.Yellow : Colors.IndianRed;
		TargetHighlightColor = color;
	}

	public void RemoveHighlight() => TargetHighlightColor = Colors.Transparent;
}
