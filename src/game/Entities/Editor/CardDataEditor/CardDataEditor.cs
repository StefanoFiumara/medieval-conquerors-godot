using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Entities.Editor.EditorStates;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class CardDataEditor : ScrollContainer
{
	[Export] private LibraryEditor _libraryEditor;

	[Export] private RichTextLabel _statusLabel;

	[Export] private Button _newButton;
	[Export] private Button _saveButton;
	[Export] private Button _deleteButton;

	[Export] private LineEdit _cardTitle;
	[Export] private TextEdit _description;

	[Export] private ImageSelector _portraitSelector;
	[Export] private CardTypeOptions _cardTypeSelector;
	[Export] private TagSelector _tagSelector;
	[Export] private AttributeOptions _newAttributeSelector;
	[Export] private Button _addAttributeButton;

	[Export] private VBoxContainer _attributesContainer;

	private StateMachine _stateMachine;
	private PackedScene _objectEditor;

	// Mutable backing fields - values read directly from UI controls
	// TODO: Should be a separate AttributeEditor with the same patterns (creating immutable attribute records?)
	private List<ICardAttribute> _dataAttributes = new();

	public int CurrentCardId { get; private set; }

	// Creates a CardData object from the current UI control values
	private CardData CreateCardData()
	{
		return new CardData
		{
			Id = CurrentCardId,
			Title = _cardTitle.Text?.Trim() ?? string.Empty,
			Description = _description.Text?.Trim() ?? string.Empty,
			ImagePath = _portraitSelector.SelectedImageUid,
			TokenImagePath = _portraitSelector.SelectedTokenUid,
			CardType = _cardTypeSelector.SelectedOption,
			Tags = _tagSelector.SelectedTags,
			Attributes = new List<ICardAttribute>(_dataAttributes)
		};
	}

	// Loads a CardData object into the UI controls
	private void Load(CardData data)
	{
		Reset();

		if (data != null)
		{
			// Populate UI controls from CardData
			CurrentCardId = data.Id;
			_cardTitle.Text = data.Title ?? string.Empty;
			_description.Text = data.Description ?? string.Empty;
			_portraitSelector.SelectedImageUid = data.ImagePath;
			_cardTypeSelector.SelectedOption = data.CardType;
			_tagSelector.SelectedTags = data.Tags;
			_dataAttributes = new List<ICardAttribute>(data.Attributes);

			foreach (var attr in data.Attributes)
				CreateAttributeEditor(attr);
		}

		if(data == null)
			_stateMachine.ChangeState(new NoDataState(this));
		else if (data.Id == 0)
			_stateMachine.ChangeState(new CreatingNewCardState(this));
		else
			_stateMachine.ChangeState(new EditingExistingCardState(this));

		_saveButton.Disabled = _stateMachine.CurrentState is NoDataState;
		_deleteButton.Disabled = _stateMachine.CurrentState is NoDataState;
	}

	public override void _EnterTree()
	{
		// TODO: If we update these methods to use Connect(), we won't need to unsubscribe in ExitTree
		//		And can move all this logic to _Ready()
		_newAttributeSelector.ItemSelected += OnNewAttributeSelected;
		_addAttributeButton.Pressed += CreateNewAttribute;
		_saveButton.Pressed += SaveCardResource;
		_newButton.Pressed += CreateNewCard;
		_deleteButton.Pressed += DeleteLoadedCard;

		// TODO: Update to use binding framework
		_portraitSelector.ImageSelected += OnPortraitSelected;
	}

	public override void _ExitTree()
	{
		_newAttributeSelector.ItemSelected -= OnNewAttributeSelected;

		_addAttributeButton.Pressed -= CreateNewAttribute;
		_saveButton.Pressed -= SaveCardResource;
		_newButton.Pressed -= CreateNewCard;
		_deleteButton.Pressed -= DeleteLoadedCard;

		_portraitSelector.ImageSelected -= OnPortraitSelected;
	}

	public override void _Ready()
	{
		_objectEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");
		_stateMachine = new StateMachine(new NoDataState(this));

		Load(null);

		// TODO: is it possible to subscribe in EnterTree? Library editor probably will not exist yet
		//		Maybe a different way of setting up this communication
		// IDEA: turn this into a signal and wire it up in the godot editor instead.
		_libraryEditor.SearchResultClicked += card => Load(card);
	}

	public void Enable()
	{
		_saveButton.Disabled = false;
		_cardTitle.Editable = true;
		_description.Editable = true;
		_cardTypeSelector.Disabled = false;
		_newAttributeSelector.Disabled = false;
		_addAttributeButton.Disabled = _newAttributeSelector.Selected == 0;
		_portraitSelector.Disabled = false;
		_tagSelector.Enable();
	}

	public void Disable()
	{
		_saveButton.Disabled = true;
		_cardTitle.Editable = false;
		_description.Editable = false;
		_cardTypeSelector.Disabled = true;
		_tagSelector.Disable();
		_newAttributeSelector.Disabled = true;
		_addAttributeButton.Disabled = true;
		_portraitSelector.Disabled = true;
	}

	public void SetStatus(string status) => _statusLabel.Text = status;

	private void Reset()
	{
		// Clear backing fields
		CurrentCardId = 0;
		_dataAttributes.Clear();

		// Clear UI controls
		_cardTitle.Text = string.Empty;
		_description.Text = string.Empty;
		_cardTypeSelector.SelectedOption = CardType.None;
		_tagSelector.SelectedTags = Tags.None;

		var attributeControls = _attributesContainer.GetChildren();

		foreach (var control in attributeControls)
			control.QueueFree();

		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
	}

	private void OnPortraitSelected()
	{
		// Portrait values are read directly from _portraitSelector in CreateCardData()
		// No need to store in backing fields
	}

	private void OnNewAttributeSelected(long itemIndex)
	{
		var selectedText = _newAttributeSelector.GetItemText((int)itemIndex);
		_addAttributeButton.Disabled = selectedText == "None" || _dataAttributes.Any(a => a.GetType().Name == selectedText);
	}

	private void CreateNewCard()
	{
		Load(new CardData());
		GD.PrintRich("New Card Data Resource Created".Purple());
	}

	private void CreateNewAttribute()
	{
		var attr = _newAttributeSelector.CreateSelected();
		_dataAttributes.Add(attr);

		CreateAttributeEditor(attr);

		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
	}

	private void CreateAttributeEditor(ICardAttribute attr)
	{
		// TODO: perhaps this should be a List Editor instead, but it is polymorphic, so we have to solve for that first.
		var editor = _objectEditor.Instantiate<ObjectEditor>();
		_attributesContainer.AddChild(editor);

		editor.Load(
			target: attr,
			customTitle: attr.GetType().Name.PrettyPrint().Replace("Attribute", string.Empty),
			propertyFilter: p => p.Name != nameof(ICardAttribute.Owner)
		);

		// TODO: Re-implement RemovePressed functionality for attributes
		// editor.RemovePressed += () =>
		// {
		// 	_attributesContainer.RemoveChild(editor);
		// 	LoadedData.Attributes.Remove(attr);
		// 	editor.QueueFree();
		// };
	}

	private void DeleteLoadedCard()
	{
		// TODO: Implement confirmation to prevent misclicks
		if (_stateMachine.CurrentState is NoDataState) return;

		using var database = new CardDatabase();
		var cardData = CreateCardData();
		database.DeleteCardData(cardData);

		Load(null);
	}

	private void SaveCardResource()
	{
		if (_stateMachine.CurrentState is NoDataState) return;

		// Create CardData from UI controls
		var dataToSave = CreateCardData();

		try
		{
			using var db = new CardDatabase();
			var savedId = db.SaveCardData(dataToSave);
			var savedData = dataToSave with { Id = savedId };
			Load(savedData);
			GD.PrintRich("Successfully saved Card Data".Green());
		}
		catch(Exception e)
		{
			GD.PrintErr("Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}
}
