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
	
	private PanelContainer _container;
	
	private CardDataEditor _editor;
	private CardLibrary _library;
	
	public override void _EnterTree()
	{
		_container = new PanelContainer { Name = "Card Data Editor"};
		CallDeferred(nameof(CreateCardDataEditor));
	}

	private void CreateCardDataEditor()
	{
		_editorScene = GD.Load<PackedScene>("res://addons/CardDataEditor/card_data_editor.tscn");
		_libraryScene = GD.Load<PackedScene>("res://addons/CardDataEditor/Library/card_library.tscn");
		
		_editor = _editorScene.Instantiate<CardDataEditor>();
		_library = _libraryScene.Instantiate<CardLibrary>();
		
		// TODO: Move the reload button somewhere else
		var reloadButton = new Button { Text = "\u267b" };
		reloadButton.TooltipText = "Reload Plugin";
		reloadButton.Pressed += OnReloadPressed;
		_editor.GetNode("%editor_vbox_container").AddChild(reloadButton);
		
		//  TODO: Move the navigation header inside its own scene and add it to the container separately so all navigation logic can go here.
		_container.AddChild(_editor);
		_container.AddChild(_library);
		
		_library.SearchResultClicked += NavigateToEditor;
		_library.EditorNavigation += NavigateToEditor;
		_editor.LibraryNavigation += NavigateToLibrary;

		_library.Visible = false;
		AddControlToDock(DockSlot.RightUl, _container);
	}

	private void NavigateToLibrary()
	{
		_library.Visible = true;
		_editor.Visible = false;
	}

	private void NavigateToEditor() => NavigateToEditor(default);
	private void NavigateToEditor(CardData card)
	{
		if (card != default) 
			_editor.LoadedData = card;
		
		_library.Visible = false;
		_editor.Visible = true;
	}
	
	private void OnReloadPressed()
	{
		_library.SearchResultClicked -= NavigateToEditor;
		_library.EditorNavigation -= NavigateToEditor;
		_editor.LibraryNavigation -= NavigateToLibrary;

		foreach (var node in _container.GetChildren()) 
			node.QueueFree();
		
		CallDeferred(nameof(CreateCardDataEditor));
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(_container);
		
		_library.SearchResultClicked -= NavigateToEditor;
		_library.EditorNavigation -= NavigateToEditor;
		_editor.LibraryNavigation -= NavigateToLibrary;
		
		_container.Free();
	}
}
#endif
