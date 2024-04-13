#if TOOLS
using Godot;
using MedievalConquerors.addons.CardDataEditor.Library;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor;

[Tool]
public partial class CardDataEditorLoader : EditorPlugin
{
	private PackedScene _editorScene;
	private PackedScene _libraryScene;
	
	private CardDataEditor _editorInstance;
	private CardLibrary _libraryInstance;
	
	public override void _EnterTree()
	{
		CallDeferred(nameof(CreateCardDataEditor));
	}

	private void CreateCardDataEditor()
	{
		_editorScene = GD.Load<PackedScene>("res://addons/CardDataEditor/card_data_editor.tscn");
		_libraryScene = GD.Load<PackedScene>("res://addons/CardDataEditor/Library/card_library.tscn");
		
		_editorInstance = _editorScene.Instantiate<CardDataEditor>();
		
		var reloadButton = new Button { Text = "Reload Plugin" };
		reloadButton.Pressed += OnReloadPressed;
		_editorInstance.GetNode("%editor_vbox_container").AddChild(reloadButton);
		
		_libraryInstance = _libraryScene.Instantiate<CardLibrary>();
		
		var container = new PanelContainer { Name = "Card Data Editor"};

		container.AddChild(_editorInstance);
		container.AddChild(_libraryInstance);
		
		_libraryInstance.SearchResultClicked += LoadCard;
		_libraryInstance.EditorNavigation += NavigateToEditor;
		_editorInstance.LibraryNavigation += NavigateToLibrary;

		_libraryInstance.Visible = false;
		AddControlToDock(DockSlot.RightUl, container);
	}

	private void LoadCard(CardData card)
	{
		_editorInstance.LoadedData = card;
		_libraryInstance.Visible = false;
		_editorInstance.Visible = true;
	}

	private void NavigateToLibrary()
	{
		_libraryInstance.Visible = true;
		_editorInstance.Visible = false;
	}
	
	private void NavigateToEditor()
	{
		_libraryInstance.Visible = false;
		_editorInstance.Visible = true;
	}
	
	private void OnReloadPressed()
	{
		_libraryInstance.SearchResultClicked -= LoadCard;
		_editorInstance?.QueueFree();
		_libraryInstance?.QueueFree();
		
		CallDeferred(nameof(CreateCardDataEditor));
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(_editorInstance);
		RemoveControlFromDocks(_libraryInstance);
		_libraryInstance.SearchResultClicked -= LoadCard;
		_libraryInstance.EditorNavigation -= NavigateToEditor;
		_editorInstance.LibraryNavigation -= NavigateToLibrary;
		_editorInstance.Free();
		_libraryInstance.Free();
	}
}
#endif
