#if TOOLS
using Godot;
using MedievalConquerors.ConquerorsPlugin.Editor;
using MedievalConquerors.ConquerorsPlugin.LibraryBrowser;
using MedievalConquerors.ConquerorsPlugin.Navigation;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.ConquerorsPlugin;

[Tool]
public partial class PluginLoader : EditorPlugin
{
	private PackedScene _editorScene;
	private PackedScene _libraryScene;
	private PackedScene _navigationScene;
	
	private VBoxContainer _container;
	
	private NavigationBar _navigation;
	private CardDataEditor _editor;
	private CardLibrary _library;
	
	public override void _EnterTree()
	{
		_container = new VBoxContainer { Name = "Card Data Editor" };
		CallDeferred(nameof(CreateCardDataEditor));
	}

	private void CreateCardDataEditor()
	{
		_editorScene = GD.Load<PackedScene>("res://addons/ConquerorsPlugin/Editor/card_data_editor.tscn");
		_libraryScene = GD.Load<PackedScene>("res://addons/ConquerorsPlugin/LibraryBrowser/card_library.tscn");
		_navigationScene = GD.Load<PackedScene>("res://addons/ConquerorsPlugin/Navigation/navigation_bar.tscn");

		_navigation = _navigationScene.Instantiate<NavigationBar>();
		_editor = _editorScene.Instantiate<CardDataEditor>();
		_library = _libraryScene.Instantiate<CardLibrary>();

		_navigation.ReloadPressed += OnReloadPressed;
		_navigation.LibraryPressed += NavigateToLibrary;
		_navigation.EditorPressed += NavigateToEditor;
		
		_container.AddChild(_navigation);
		_container.AddChild(_library);
		_container.AddChild(_editor);
		
		
		_library.SearchResultClicked += NavigateToEditor;

		_editor.Visible = false;
		AddControlToDock(DockSlot.RightUl, _container);
	}

	private void NavigateToLibrary()
	{
		_navigation.ActivePanel = EditorPanel.Library;
		_library.Visible = true;
		_editor.Visible = false;
	}

	private void NavigateToEditor() => NavigateToEditor(default);
	private void NavigateToEditor(CardData card)
	{
		if (card != default) 
			_editor.LoadedData = card;

		_navigation.ActivePanel = EditorPanel.CardData;
		_library.Visible = false;
		_editor.Visible = true;
	}
	
	private void OnReloadPressed()
	{
		_library.SearchResultClicked -= NavigateToEditor;

		foreach (var node in _container.GetChildren()) 
			node.QueueFree();
		
		CallDeferred(nameof(CreateCardDataEditor));
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(_container);
		
		_library.SearchResultClicked -= NavigateToEditor;
		_container.Free();
	}
}
#endif
