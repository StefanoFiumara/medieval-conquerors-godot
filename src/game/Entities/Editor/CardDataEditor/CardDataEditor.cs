using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.DataBinding;
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

	private CardData _loadedData;
	private StateMachine _stateMachine;
	private PackedScene _objectEditor;

	// Mutable backing fields for data binding
	private int _dataId;
	private string _dataTitle;
	private string _dataDescription;
	private string _dataImagePath;
	private string _dataTokenImagePath;
	private CardType _dataCardType;
	private Tags _dataTags;
	private List<ICardAttribute> _dataAttributes = new();

	public CardData LoadedData
	{
		get => _loadedData;
		private set
		{
			Reset();
			_loadedData = value;

			if (value != null)
			{
				// Populate mutable backing fields from CardData
				_dataId = value.Id;
				_dataTitle = value.Title;
				_dataDescription = value.Description;
				_dataImagePath = value.ImagePath;
				_dataTokenImagePath = value.TokenImagePath;
				_dataCardType = value.CardType;
				_dataTags = value.Tags;
				_dataAttributes = new List<ICardAttribute>(value.Attributes);

				// Bind UI controls to mutable backing fields
				_cardTitle.Bind(this, editor => editor._dataTitle);
				_description.Bind(this, editor => editor._dataDescription);
				_cardTypeSelector.Bind(this, editor => editor._dataCardType);
				_tagSelector.Bind(this, editor => editor._dataTags);

				// TODO: data binding for portrait selector
				_portraitSelector.SelectedImageUid = _dataImagePath;

				foreach (var attr in value.Attributes)
					CreateAttributeEditor(attr);
			}

			if(value == null)
				_stateMachine.ChangeState(new NoDataState(this));
			else if (value.Id == 0)
				_stateMachine.ChangeState(new CreatingNewCardState(this));
			else
				_stateMachine.ChangeState(new EditingExistingCardState(this));

			_saveButton.Disabled = _loadedData == null;
			_deleteButton.Disabled = _loadedData == null;
		}
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

		LoadedData = null;

		// TODO: is it possible to subscribe in EnterTree? Library editor probably will not exist yet
		//		Maybe a different way of setting up this communication
		// IDEA: turn this into a signal and wire it up in the godot editor instead.
		_libraryEditor.SearchResultClicked += card => LoadedData = card;
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
		// Clear mutable backing fields
		_dataId = 0;
		_dataTitle = string.Empty;
		_dataDescription = string.Empty;
		_dataImagePath = string.Empty;
		_dataTokenImagePath = string.Empty;
		_dataCardType = CardType.None;
		_dataTags = Tags.None;
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

	// TODO: Better way to bind this data to loaded data?
	private void OnPortraitSelected()
	{
		if (_loadedData != null)
		{
			_dataImagePath = _portraitSelector.SelectedImageUid;
			_dataTokenImagePath = _portraitSelector.SelectedTokenUid;
		}
	}

	private void OnNewAttributeSelected(long itemIndex)
	{
		var selectedText = _newAttributeSelector.GetItemText((int)itemIndex);
		_addAttributeButton.Disabled = selectedText == "None" || _dataAttributes.Any(a => a.GetType().Name == selectedText);
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
		using var database = new CardDatabase();
		database.DeleteCardData(LoadedData);

		LoadedData = null;
		_stateMachine.ChangeState(new NoDataState(this));
	}

	private void SaveCardResource()
	{
		if (LoadedData == null) return;

		// Build CardData from mutable backing fields
		var dataToSave = new CardData
		{
			Id = _dataId,
			Title = _dataTitle?.Trim() ?? string.Empty,
			Description = _dataDescription?.Trim() ?? string.Empty,
			ImagePath = _dataImagePath,
			TokenImagePath = _dataTokenImagePath,
			CardType = _dataCardType,
			Tags = _dataTags,
			Attributes = _dataAttributes
		};

		try
		{
			using var db = new CardDatabase();
			var savedId = db.SaveCardData(dataToSave);
			LoadedData = dataToSave with { Id = savedId };
			GD.PrintRich("Successfully saved Card Data".Green());

			_stateMachine.ChangeState(new EditingExistingCardState(this));
		}
		catch(Exception e)
		{
			GD.PrintErr("Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}
}
