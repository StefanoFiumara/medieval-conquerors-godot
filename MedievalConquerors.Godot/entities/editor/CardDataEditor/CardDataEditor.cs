using System;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Entities.Editor.EditorStates;

namespace MedievalConquerors.Entities.Editor.CardDataEditor;

public partial class CardDataEditor : ScrollContainer
{
	[Export] private LibraryEditor.LibraryEditor _libraryEditor;

	// TODO: Remove internal exports, link children via node paths/uids
	[Export] private RichTextLabel _panelTitle;

	[Export] private Button _newButton;
	[Export] private Button _saveButton;
	[Export] private Button _deleteButton;

	[Export] private LineEdit _cardTitle;
	[Export] private TextEdit _description;

	[Export] private ImageSelector _imageSelector;
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
		private set
		{
			_loadedData = value;
			Reset();
			if (value != null)
			{
				_cardTitle.Text = _loadedData.Title;
				_description.Text = _loadedData.Description;
				_cardTypeOptions.SelectedOption = _loadedData.CardType;
				_tagOptions.SelectedTags = _loadedData.Tags;
				_imageSelector.SelectedImageUid = _loadedData.ImagePath;

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

	public override void _Ready()
	{
		_attributeEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");

		LoadedData = null;
		_stateMachine = new StateMachine(new NoDataState(this));
		_libraryEditor.SearchResultClicked += card => LoadedData = card;
	}

	public override void _EnterTree()
	{
		_attributeSelector.ItemSelected += OnAttributeSelectorItemSelected;
		_addAttributeButton.Pressed += CreateNewAttribute;
		_saveButton.Pressed += SaveCardResource;
		_newButton.Pressed += CreateNewCard;
		_deleteButton.Pressed += DeleteLoadedCard;
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
		_imageSelector.Disabled = false;
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
		_imageSelector.Disabled = true;
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
		_cardTypeOptions.SelectedOption = CardType.None;
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
		var editor = _attributeEditor.Instantiate<AttributeEditor.AttributeEditor>();
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
		using var database = new CardDatabase();
		database.DeleteCardData(LoadedData);

		LoadedData = null;
		_stateMachine.ChangeState(new NoDataState(this));
	}

	private void UpdateLoadedResourceFromEditor()
	{
		// TODO: set up auto update of these basic properties so that this method can be removed
		LoadedData.Title = _cardTitle.Text.Trim();
		LoadedData.Description = _description.Text.Trim();
		LoadedData.CardType = _cardTypeOptions.SelectedOption;
		LoadedData.Tags = _tagOptions.SelectedTags;
		LoadedData.ImagePath = _imageSelector.SelectedImageUid;
		LoadedData.TokenImagePath = _imageSelector.SelectedTokenUid;
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
			GD.PrintErr("Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}

	public override void _ExitTree()
	{
		_attributeSelector.ItemSelected -= OnAttributeSelectorItemSelected;
		_addAttributeButton.Pressed -= CreateNewAttribute;
		_saveButton.Pressed -= SaveCardResource;
		_newButton.Pressed -= CreateNewCard;
		_deleteButton.Pressed -= DeleteLoadedCard;
	}
}
