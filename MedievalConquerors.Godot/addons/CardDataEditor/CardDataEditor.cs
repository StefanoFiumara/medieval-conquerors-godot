using System;
using Godot;
using MedievalConquerors.Addons.CardDataEditor.Controls;
using MedievalConquerors.Addons.CardDataEditor.UIStates;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Addons.CardDataEditor;

[Tool]
public partial class CardDataEditor : VBoxContainer
{
	private CardData _loadedData;
	private StateMachine _stateMachine;
	private bool _isDirty;

	// UI Components
	public RichTextLabel PanelTitle { get; private set; }

	public Button NewButton { get; private set; }
	public Button SaveButton { get; private set; }
	
	public TextEdit CardTitle { get; private set; }
	public TextEdit Description { get; private set; }
	public CardTypeOptions CardType { get; private set; }
	public TagOptions Tags { get; private set; }

	public bool IsDirty
	{
		get => _isDirty;
		set
		{
			_isDirty = value;
			SaveButton.Disabled = !_isDirty;
		}
	}

	public CardData LoadedData
	{
		get => _loadedData;
		set
		{
			_loadedData = value;
			if (value != null)
			{
				CardTitle.Text = _loadedData.Title;
				Description.Text = _loadedData.Description;
				CardType.SelectedCardType = _loadedData.CardType;
				Tags.SelectedTags = _loadedData.Tags;
				// TODO: Load Attributes from resource
				// _attributes.Load(_loadedCardResource);
			}
			
			SaveButton.Disabled = _loadedData == null;
		}
	}
	
	public override void _Ready()
	{
		// TODO: use "%" in scene to give relevant nodes unique names, so we don't have to constantly update this when their place in the hierarchy changes
		PanelTitle = GetNode<RichTextLabel>("%currently_editing");
		SaveButton = GetNode<Button>("%save_btn");
		NewButton = GetNode<Button>("%new_btn");
		CardTitle = GetNode<TextEdit>("%title_edit");
		Description = GetNode<TextEdit>("%desc_edit");
		CardType = GetNode<CardTypeOptions>("%card_type_selector");
		Tags = GetNode<TagOptions>("%tags_grid");

		LoadedData = null;
		_stateMachine = new StateMachine(new NoDataState(this));

		// TODO: Reference to attribute editor, to pull resulting resources for card attributes
		// _attributes = GetNode<AttributeOptions>("attribute_editor/attr_selector");

		CardTitle.LinesEditedFrom += (_, _) => IsDirty = CardTitle.Text != LoadedData?.Title;
		Description.LinesEditedFrom += (_, _) => IsDirty = Description.Text != LoadedData?.Description;
		CardType.ItemSelected += _ => IsDirty = CardType.SelectedCardType != LoadedData?.CardType;
		Tags.TagsChanged += () => IsDirty = Tags.SelectedTags != LoadedData?.Tags;
		
		
		SaveButton.Pressed += SaveCardResource;
		NewButton.Pressed += CreateNewCard;
	}

	private void CreateNewCard()
	{
		LoadedData = new CardData();
		_stateMachine.ChangeState(new CreatingNewCardState(this));
		GD.Print("New Card Data Resource Created");
	}

	private void UpdateLoadedResourceFromEditor()
	{
		LoadedData.Title = CardTitle.Text.Trim();
		LoadedData.Description = Description.Text.Trim();
		LoadedData.CardType = CardType.SelectedCardType;
		LoadedData.Tags = Tags.SelectedTags;
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
			GD.PrintRich("Successfully save Card Data".Green());
			
			_stateMachine.ChangeState(new EditingExistingCardState(this));
		}
		catch(Exception e)
		{
			GD.PrintErr($"Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}
}
