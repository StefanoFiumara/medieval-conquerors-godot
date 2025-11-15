using System;
using System.Collections.Generic;
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

	// TODO: Refactor out state machine, we probably don't need it.
	private StateMachine _stateMachine;


	// TODO: Make private
	// TODO: If we make this a nullable int we can derive the status from it and manage state transitions in the setter
	// 		Or use something like int.MinValue to represent the NoData state.
	public int CurrentCardId { get; private set; }

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
			// TODO: Call AttributesEditor.CreateAttributes() instead
			// Attributes = _attributesEditor.CreateAttributes()
		};
	}

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

			// TODO: Delegate loading attributes to Attributes Editor
			// _attributesEditor.Load(data.Attributes)
			// _dataAttributes = new List<ICardAttribute>(data.Attributes);
			// foreach (var attr in data.Attributes)
			// 	CreateAttributeEditor(attr);
		}

		// TODO: Refactor out state machine stuff, figure out what to do with the save/delete buttons here.
		if(data == null)
			_stateMachine.ChangeState(new NoDataState(this));
		else if (data.Id == 0)
			_stateMachine.ChangeState(new CreatingNewCardState(this));
		else
			_stateMachine.ChangeState(new EditingExistingCardState(this));

		_saveButton.Disabled = _stateMachine.CurrentState is NoDataState;
		_deleteButton.Disabled = _stateMachine.CurrentState is NoDataState;
	}

	public override void _Ready()
	{
		_stateMachine = new StateMachine(new NoDataState(this));

		_saveButton.Connect(BaseButton.SignalName.Pressed, Callable.From(SaveCardResource));
		_newButton.Connect(BaseButton.SignalName.Pressed, Callable.From(CreateNewCard));
		_deleteButton.Connect(BaseButton.SignalName.Pressed, Callable.From(DeleteLoadedCard));

		// TODO: is it possible to subscribe in EnterTree? Library editor probably will not exist yet
		//		Maybe a different way of setting up this communication
		// IDEA: turn this into a signal and wire it up in the godot editor instead.
		// IDEA: move this logic into a parent script that can reference both the library and the card editors.
		_libraryEditor.SearchResultClicked += Load;
	}

	public void Enable()
	{
		_saveButton.Disabled = false;
		_cardTitle.Editable = true;
		_description.Editable = true;
		_cardTypeSelector.Disabled = false;
		// TODO: enable attributes editor
		// _attributesEditor.Enable()

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
		_portraitSelector.Disabled = true;
		// TODO: disable attributes editor
		// _attributesEditor.Disable()
	}

	// TODO: maybe derive status off of CurrentCardId?
	public void SetStatus(string status) => _statusLabel.Text = status;

	private void Reset()
	{
		// Clear backing fields
		CurrentCardId = int.MinValue;

		// Clear UI controls
		_cardTitle.Text = string.Empty;
		_description.Text = string.Empty;
		_portraitSelector.SelectedImageUid = ImageSelector.None;
		_cardTypeSelector.SelectedOption = CardType.None;
		_tagSelector.SelectedTags = Tags.None;

		// TODO: reset the attribute editor
		// _attributesEditor.Reset();
	}


	private void CreateNewCard()
	{
		// TODO: Set the state here without creating a new CardData, since it will get discarded anyway.
		Load(new CardData());
		GD.PrintRich("New Card Data Resource Created".Purple());
	}

	private void SaveCardResource()
	{
		if (_stateMachine.CurrentState is NoDataState) return;

		try
		{
			var dataToSave = CreateCardData();

			using var db = new CardDatabase();
			var savedId = db.SaveCardData(dataToSave);
			var savedData = dataToSave with { Id = savedId };
			// TODO: Should we reload here or just update the CurrentCardId?
			Load(savedData);
			GD.PrintRich("Successfully saved Card Data".Green());
		}
		catch(Exception e)
		{
			GD.PrintErr("Failed to save Card Data");
			GD.PrintErr(e.ToString());
		}
	}

	private void DeleteLoadedCard()
	{
		// TODO: Implement confirmation to prevent misclicks
		if (_stateMachine.CurrentState is NoDataState) return;

		using var database = new CardDatabase();
		var cardData = CreateCardData();
		// TODO: Should this method just take an ID instead of an entire object?
		database.DeleteCardData(cardData);

		Load(null);
	}
}
