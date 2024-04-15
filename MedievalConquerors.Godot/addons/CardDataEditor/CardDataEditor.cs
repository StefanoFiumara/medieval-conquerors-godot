using System;
using System.Linq;
using Godot;
using MedievalConquerors.Addons.CardDataEditor.Controls;
using MedievalConquerors.Addons.CardDataEditor.Data;
using MedievalConquerors.Addons.CardDataEditor.UIStates;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Extensions;
using AttributeEditor = MedievalConquerors.Addons.CardDataEditor.Attributes.AttributeEditor;

namespace MedievalConquerors.Addons.CardDataEditor;

[Tool]
public partial class CardDataEditor : ScrollContainer
{
	[Export] private RichTextLabel _panelTitle;
	
	[Export] private Button _newButton;
	[Export] private Button _saveButton;
	
	[Export] private LineEdit _cardTitle;
	[Export] private TextEdit _description;
	
	[Export] private CardTypeOptions _cardTypeOptions;
	[Export] private TagOptions _tagOptions;
	[Export] private AttributeOptions _attributeSelector;
	[Export] private Button _addAttributeButton;
	
	[Export] private VBoxContainer _attributesContainer;
	
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
		
		LoadedData = null;
		_stateMachine = new StateMachine(new NoDataState(this));
	}

	public override void _EnterTree()
	{
		_attributeSelector.ItemSelected += OnAttributeSelectorItemSelected;
		_addAttributeButton.Pressed += CreateNewAttribute;
		_saveButton.Pressed += SaveCardResource;
		_newButton.Pressed += CreateNewCard;
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
		editor.RemovePressed += () =>
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
	}
}
