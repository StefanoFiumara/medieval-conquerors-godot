using System;
using Godot;
using MedievalConquerors.addons.CardDataEditor.Controls;
using MedievalConquerors.Addons.CardDataEditor.Controls;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor;

[Tool]
public partial class CardDataEditor : VBoxContainer
{
	// UI Components
	private RichTextLabel _currentlyEditing;
	private Button _newButton;
	private Button _saveButton;
	private Button _loadButton;
	private TextEdit _title;
	private TextEdit _description;
	private CardTypeOptions _cardType;
	private TagOptions _tags;

	private CardData _loadedData;

	private CardData LoadedData
	{
		get => _loadedData;
		set
		{
			_loadedData = value;
			if (value != null)
			{
				_title.Text = _loadedData.Title;
				_description.Text = _loadedData.Description;
				_cardType.SelectedCardType = _loadedData.CardType;
				_tags.SelectedTags = _loadedData.Tags;
				// TODO: Load Attributes from resource
				// _attributes.Load(_loadedCardResource);
			}
			
			_saveButton.Disabled = _loadedData == null;
		}
	}

	public override void _Ready()
	{
		// TODO: use "%" in scene to give relevant nodes unique names, so we don't have to constantly update this when their place in the hierarchy changes
		_currentlyEditing = GetNode<RichTextLabel>("currently_editing");
		_saveButton = GetNode<Button>("save_btn");
		_loadButton = GetNode<Button>("save_load/load_btn");
		_newButton = GetNode<Button>("save_load/new_btn");
		_title = GetNode<TextEdit>("title_editor/title_edit");
		_description = GetNode<TextEdit>("desc_editor/desc_edit");
		_cardType = GetNode<CardTypeOptions>("card_type_editor/card_type_selector");
		_tags = GetNode<TagOptions>("Tags/tags_grid");

		LoadedData = null;

		// TODO: Reference to attribute editor, to pull resulting resources for card attributes
		// _attributes = GetNode<AttributeOptions>("attribute_editor/attr_selector");
		
		_saveButton.Pressed += SaveCardResource;
		_newButton.Pressed += CreateNewCard;
	}

	private void CreateNewCard()
	{
		LoadedData = new CardData();
		GD.Print("New Card Data Resource Created");
	}

	private void UpdateLoadedResourceFromEditor()
	{
		LoadedData.Title = _title.Text.Trim();
		LoadedData.Description = _description.Text.Trim();
		LoadedData.CardType = _cardType.SelectedCardType;
		LoadedData.Tags = _tags.SelectedTags;
		// TODO: Update Attributes
	}

	private void SaveCardResource()
	{
		if (LoadedData == null) return;
		
		UpdateLoadedResourceFromEditor();

		try
		{
			using var db = new CardDatabase();
			LoadedData.Id = db.SaveCardData(LoadedData);
			GD.PrintRich($"Successfully save Card Data".Green());
		}
		catch(Exception e)
		{
			GD.PrintErr($"Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}
}
