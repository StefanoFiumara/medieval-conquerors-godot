using System;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.addons.CardDataEditor.Library;

[Tool]
public partial class CardResult : PanelContainer
{
	public event Action<CardData> OnSelected;

	private Button _editButton;
	private Label _titleLabel;
	private Label _descLabel;

	private CardData _cardData;
	
	public override void _Ready()
	{
		_editButton = GetNode<Button>("%edit_button");
		_titleLabel = GetNode<Label>("%card_title");
		_descLabel = GetNode<Label>("%card_desc");

		_editButton.Pressed += OnCardSelected;
	}

	public void Load(CardData card)
	{
		_cardData = card;
		_titleLabel.Text = card.Title;
		_descLabel.Text = card.Description;
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
