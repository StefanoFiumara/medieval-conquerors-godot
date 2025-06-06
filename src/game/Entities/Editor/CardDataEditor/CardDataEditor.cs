using System;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Entities.Editor.EditorStates;
using MedievalConquerors.Entities.Editor.PropertyEditors;
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
	private PackedScene _attributeEditor;

	public CardData LoadedData
	{
		get => _loadedData;
		private set
		{
			_loadedData = null;
			Reset();
			_loadedData = value;

			if (value != null)
			{
				_cardTitle.Text = _loadedData.Title;
				_description.Text = _loadedData.Description;

				_cardTypeSelector.SelectedOption = _loadedData.CardType;

				_tagSelector.SelectedTags = _loadedData.Tags;
				_portraitSelector.SelectedImageUid = _loadedData.ImagePath;

				foreach (var attr in value.Attributes)
					CreateAttributeEditor(attr);

				GD.Print($"Updating Editor to card data with ID: {value.Id}");
				if (value.Id == 0)
					_stateMachine.ChangeState(new CreatingNewCardState(this));
				else
					_stateMachine.ChangeState(new EditingExistingCardState(this));
			}

			_saveButton.Disabled = _loadedData == null;
			_deleteButton.Disabled = _loadedData == null;
		}
	}

	public override void _EnterTree()
	{
		_newAttributeSelector.ItemSelected += OnNewAttributeSelected;

		_addAttributeButton.Pressed += CreateNewAttribute;
		_saveButton.Pressed += SaveCardResource;
		_newButton.Pressed += CreateNewCard;
		_deleteButton.Pressed += DeleteLoadedCard;

		_cardTitle.TextChanged += CardTitleChanged;
		_description.TextChanged += CardDescriptionChanged;

		_cardTypeSelector.ItemSelected += OnCardTypeSelected;
		_tagSelector.TagsChanged += OnTagsChanged;

		_portraitSelector.ImageSelected += OnPortraitSelected;
	}

	public override void _ExitTree()
	{
		_newAttributeSelector.ItemSelected -= OnNewAttributeSelected;

		_addAttributeButton.Pressed -= CreateNewAttribute;
		_saveButton.Pressed -= SaveCardResource;
		_newButton.Pressed -= CreateNewCard;
		_deleteButton.Pressed -= DeleteLoadedCard;

		_cardTitle.Bind(LoadedData, data => data.Title);

		_cardTitle.TextChanged -= CardTitleChanged;
		_description.TextChanged -= CardDescriptionChanged;

		_cardTypeSelector.ItemSelected -= OnCardTypeSelected;
		_tagSelector.TagsChanged -= OnTagsChanged;

		_portraitSelector.ImageSelected -= OnPortraitSelected;
	}

	public override void _Ready()
	{
		_attributeEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

		LoadedData = null;
		_stateMachine = new StateMachine(new NoDataState(this));

		// TODO: is it possible to subscribe in EnterTree? Library editor probably will not exist yet
		//		Maybe a different way of setting up this communication
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
		// we may be able to remove some of this
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
	private void CardTitleChanged(string newTitle)
	{
		if (_loadedData != null)
			_loadedData.Title = newTitle;
	}

	private void CardDescriptionChanged()
	{
		if(_loadedData != null)
			_loadedData.Description = _description.Text;
	}

	private void OnCardTypeSelected(long index)
	{
		if (_loadedData != null)
			_loadedData.CardType = _cardTypeSelector.SelectedOption;
	}

	private void OnTagsChanged()
	{
		if (_loadedData != null)
			_loadedData.Tags = _tagSelector.SelectedTags;
	}

	private void OnPortraitSelected()
	{
		if (_loadedData != null)
		{
			_loadedData.ImagePath = _portraitSelector.SelectedImageUid;
			_loadedData.TokenImagePath = _portraitSelector.SelectedTokenUid;
		}
	}

	private void OnNewAttributeSelected(long itemIndex)
	{
		var selectedText = _newAttributeSelector.GetItemText((int)itemIndex);
		_addAttributeButton.Disabled = selectedText == "None" || LoadedData?.Attributes.Any(a => a.GetType().Name == selectedText) == true;
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
		LoadedData.Attributes.Add(attr);

		CreateAttributeEditor(attr);

		_newAttributeSelector.Select(0);
		OnNewAttributeSelected(0);
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

		LoadedData.Title = LoadedData.Title.Trim();
		LoadedData.Description = LoadedData.Description.Trim();

		try
		{
			using var db = new CardDatabase();
			LoadedData.Id = db.SaveCardData(LoadedData);
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
