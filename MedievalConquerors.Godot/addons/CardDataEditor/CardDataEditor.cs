using System;
using System.Linq;
using Godot;
using MedievalConquerors.Addons.CardDataEditor.Controls;
using MedievalConquerors.Addons.CardDataEditor.UIStates;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Extensions;
using AttributeEditor = MedievalConquerors.addons.CardDataEditor.Attributes.AttributeEditor;

namespace MedievalConquerors.Addons.CardDataEditor;

[Tool]
public partial class CardDataEditor : ScrollContainer
{
	public event Action LibraryNavigation;
	
	private RichTextLabel _panelTitle;
	
	private Button _newButton;
	private Button _saveButton;
	private Button _libraryButton;
	private Button _addAttributeButton;
	
	private LineEdit _cardTitle;
	private TextEdit _description;
	
	private CardTypeOptions _cardTypeOptions;
	private TagOptions _tagOptions;
	private AttributeOptions _attributeSelector;
	
	private VBoxContainer _attributesContainer;
	
	private CardData _loadedData;
	private StateMachine _stateMachine;
	private PackedScene _attributeEditor;

	public CardData LoadedData
	{
		get => _loadedData;
		set
		{
			_loadedData = value;
			if (value != null)
			{
				Reset();
				_cardTitle.Text = _loadedData.Title;
				_description.Text = _loadedData.Description;
				_cardTypeOptions.SelectedCardType = _loadedData.CardType;
				_tagOptions.SelectedTags = _loadedData.Tags;

				foreach (var attr in value.Attributes) 
					CreateAttributeEditor(attr);

				if (value.Id == default)
					_stateMachine.ChangeState(new CreatingNewCardState(this));
				else
					_stateMachine.ChangeState(new EditingExistingCardState(this));
			}
			
			_saveButton.Disabled = _loadedData == null;
		}
	}

	public override void _Ready()
	{	
		_attributeEditor = GD.Load<PackedScene>("res://addons/CardDataEditor/Attributes/attribute_editor.tscn");
		
		_panelTitle = GetNode<RichTextLabel>("%currently_editing");
		_saveButton = GetNode<Button>("%save_btn");
		_newButton = GetNode<Button>("%new_btn");
		_libraryButton = GetNode<Button>("%library_nav_btn");
		_cardTitle = GetNode<LineEdit>("%title_edit");
		_description = GetNode<TextEdit>("%desc_edit");
		_cardTypeOptions = GetNode<CardTypeOptions>("%card_type_selector");
		_tagOptions = GetNode<TagOptions>("%tags_grid");
		_attributeSelector = GetNode<AttributeOptions>("%attr_selector");
		_addAttributeButton = GetNode<Button>("%add_attr_btn");
		_attributesContainer = GetNode<VBoxContainer>("%attributes_container");
		
		LoadedData = null;
		_stateMachine = new StateMachine(new NoDataState(this));
		
		_attributeSelector.ItemSelected += OnAttributeSelectorItemSelected;
		_addAttributeButton.Pressed += CreateNewAttribute;
		_saveButton.Pressed += SaveCardResource;
		_newButton.Pressed += CreateNewCard;
		_libraryButton.Pressed += OnLibraryNavigation;
	}

	private void OnLibraryNavigation()
	{
		LibraryNavigation?.Invoke();
	}

	public void SetTitle(string title)
	{
		_panelTitle.Text = title;
	}

	public void Enable()
	{
		_saveButton.Disabled = false;
		_cardTitle.Editable = true;
		_description.Editable = true;
		_cardTypeOptions.Disabled = false;
		_attributeSelector.Disabled = false;
		_addAttributeButton.Disabled = _attributeSelector.Selected == 0;
		_tagOptions.Enable();
	}

	public void Disable()
	{
		_saveButton.Disabled = true;
		_cardTitle.Editable = false;
		_description.Editable = false;
		_cardTypeOptions.Disabled = true;
		_tagOptions.Disable();
		_attributeSelector.Disabled = true;
		_addAttributeButton.Disabled = true;
	}

	private void OnAttributeSelectorItemSelected(long i)
	{
		var selectedText = _attributeSelector.GetItemText((int)i);
		_addAttributeButton.Disabled = selectedText == "None" || LoadedData?.Attributes.Any(a => a.GetType().Name == selectedText) == true;
	}

	private void Reset()
	{
		_cardTitle.Text = string.Empty;
		_description.Text = string.Empty;
		_cardTypeOptions.SelectedCardType = CardType.None;
		_tagOptions.SelectedTags = Tags.None;

		var attributeControls = _attributesContainer.GetChildren();
		
		foreach (var control in attributeControls)
			control.QueueFree();
		
		_attributeSelector.Select(0);
		OnAttributeSelectorItemSelected(0);
	}
	
	private void CreateNewCard()
	{
		LoadedData = new CardData();
		_stateMachine.ChangeState(new CreatingNewCardState(this));
		Reset();
		GD.PrintRich("New Card Data Resource Created".Purple());
	}

	private void CreateNewAttribute()
	{
		var attr = _attributeSelector.CreateSelected();
		LoadedData.Attributes.Add(attr);

		CreateAttributeEditor(attr);
		
		_attributeSelector.Select(0);
		OnAttributeSelectorItemSelected(0);
	}
	
	private void CreateAttributeEditor(ICardAttribute attr)
	{
		var editor = _attributeEditor.Instantiate<AttributeEditor>();
		_attributesContainer.AddChild(editor);
		
		editor.Load(attr);
		editor.RemoveButton.Pressed += () =>
		{
			_attributesContainer.RemoveChild(editor);
			LoadedData.Attributes.Remove(attr);
			editor.QueueFree();
		};
	}

	private void UpdateLoadedResourceFromEditor()
	{
		// TODO: set up auto update of these basic properties so that this method can be removed
		LoadedData.Title = _cardTitle.Text.Trim();
		LoadedData.Description = _description.Text.Trim();
		LoadedData.CardType = _cardTypeOptions.SelectedCardType;
		LoadedData.Tags = _tagOptions.SelectedTags;
		//NOTE: Attributes are automatically updated
	}

	private void SaveCardResource()
	{
		if (LoadedData == null) return;
		
		UpdateLoadedResourceFromEditor();

		try
		{
			using var db = new CardDatabase();
			LoadedData.Id = db.SaveCardData(LoadedData);
			GD.PrintRich("Successfully saved Card Data".Green());
			
			_stateMachine.ChangeState(new EditingExistingCardState(this));
		}
		catch(Exception e)
		{
			GD.PrintErr($"Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}

	public override void _ExitTree()
	{
		_attributeSelector.ItemSelected -= OnAttributeSelectorItemSelected;
		_addAttributeButton.Pressed -= CreateNewAttribute;
		_saveButton.Pressed -= SaveCardResource;
		_newButton.Pressed -= CreateNewCard;
		_libraryButton.Pressed -= OnLibraryNavigation;
	}
}
