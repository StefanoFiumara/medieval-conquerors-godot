using System;
using Godot;
using Godot.Collections;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor.Library;

[Tool]
public partial class CardResult : PanelContainer
{
	public event Action<CardData> OnSelected;

	[Export] private Button _editButton;
	[Export] private Label _titleLabel;
	[Export] private Label _cardTypeLabel;
	[Export] private Label _descLabel;

	private CardData _cardData;
	
	public override void _EnterTree()
	{
		_editButton.Pressed += OnCardSelected;
	}

	public void Load(CardData card)
	{
		_cardData = card;
		_titleLabel.Text = card.Title;
		_cardTypeLabel.Text = $"{card.CardType}";
		_descLabel.Text = card.Description;

		
		var titleColor = _cardData.CardType switch
		{
			CardType.None => Colors.Black,
			CardType.Building => Colors.DarkOliveGreen,
			CardType.Unit => Colors.Blue,
			CardType.Technology => Colors.DarkViolet,
			_ => Colors.Black
		};
		
		_titleLabel.AddThemeColorOverride("font_color", titleColor);
		_cardTypeLabel.AddThemeColorOverride("font_color", titleColor);
	}

	private void OnCardSelected()
	{
		OnSelected?.Invoke(_cardData);
	}

	public override void _ExitTree()
	{
		_editButton.Pressed -= OnCardSelected;
	}
}
