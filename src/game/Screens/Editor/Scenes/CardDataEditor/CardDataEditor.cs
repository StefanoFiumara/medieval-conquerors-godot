using System;
using System.Linq;
using Godot;
using MedievalConquerors.Editors.ListEditor;
using MedievalConquerors.Editors.CustomEditors;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Engine.StateManagement;
using MedievalConquerors.Screens.Editor.Scenes.CardDataEditor.EditorStates;

namespace MedievalConquerors.Screens.Editor.Scenes.CardDataEditor;

public partial class CardDataEditor : PanelContainer
{
	// TODO: Remove Exports for internal components, wire up in _Ready instead.
	[Export] private LibraryEditor.LibraryEditor _libraryEditor;

	[Export] private RichTextLabel _statusLabel;

	[Export] private Button _newButton;
	[Export] private Button _saveButton;
	[Export] private Button _deleteButton;

	[Export] private LineEdit _cardTitle;
	[Export] private TextEdit _description;
	[Export] private ImageSelector _portraitSelector;
	[Export] private CardTypeOptions _cardTypeSelector;
	[Export] private TagSelector _tagSelector;
	// TODO: Use ListEditor Interface after removing [Export]
	[Export] private AttributesEditor _attributesEditor;

	private StateMachine _stateMachine;
	private int _currentCardId = int.MinValue;

	public int CurrentCardId
	{
		get => _currentCardId;
		private set
		{
			_currentCardId = value;
			switch (_currentCardId)
			{
				case 0:
					_stateMachine.ChangeState(new CreatingNewCardState(this));
					break;
				case > 0:
					_stateMachine.ChangeState(new EditingExistingCardState(this));
					break;
				case < 0:
					_stateMachine.ChangeState(new NoDataState(this));
					break;
			}

			_saveButton.Disabled = _stateMachine.CurrentState is NoDataState;
			_deleteButton.Disabled = _stateMachine.CurrentState is NoDataState;
		}
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

	private void Load(CardData data)
	{
		if (data == null)
		{
			CurrentCardId = int.MinValue;
			Reset();
		}
		else
		{
			// Populate UI controls from CardData
			CurrentCardId = data.Id;
			_cardTitle.Text = data.Title ?? string.Empty;
			_description.Text = data.Description ?? string.Empty;
			_portraitSelector.SelectedImageUid = data.ImagePath;
			_cardTypeSelector.SelectedOption = data.CardType;
			_tagSelector.SelectedTags = data.Tags;
			_attributesEditor.Load("Attributes:", data.Attributes.ToList(), allowDelete: false);
		}
	}

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
			Attributes = _attributesEditor.Create()
		};
	}

	public void SetStatus(string status) => _statusLabel.Text = status;

	public void Enable()
	{
		_saveButton.Disabled = false;
		_cardTitle.Editable = true;
		_description.Editable = true;
		_cardTypeSelector.Disabled = false;
		_tagSelector.Enable();
		_portraitSelector.Disabled = false;
		_attributesEditor.Enable();
	}

	public void Disable()
	{
		_saveButton.Disabled = true;
		_cardTitle.Editable = false;
		_description.Editable = false;
		_cardTypeSelector.Disabled = true;
		_tagSelector.Disable();
		_portraitSelector.Disabled = true;
		_attributesEditor.Disable();
	}

	private void Reset()
	{
		// Clear UI controls
		_cardTitle.Text = string.Empty;
		_description.Text = string.Empty;
		_portraitSelector.SelectedImageUid = ImageSelector.None;
		_cardTypeSelector.SelectedOption = CardType.None;
		_tagSelector.SelectedTags = Tags.None;
		_attributesEditor.Reset();
	}


	private void CreateNewCard()
	{
		CurrentCardId = 0;
		Reset();
	}

	private void SaveCardResource()
	{
		if (_stateMachine.CurrentState is NoDataState) return;
		var dataToSave = CreateCardData();

		try
		{
			using var db = new CardDatabase();
			CurrentCardId = db.SaveCardData(dataToSave);
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
		database.DeleteCardData(CurrentCardId);

		CurrentCardId = int.MinValue;
		Reset();
	}
}
