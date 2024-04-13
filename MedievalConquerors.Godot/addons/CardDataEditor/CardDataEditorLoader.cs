#if TOOLS
using Godot;
using MedievalConquerors.addons.CardDataEditor.Library;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor;

public enum PanelType
{
	Editor, Library
}

[Tool]
public partial class CardDataEditorLoader : EditorPlugin
{
	private PackedScene _editorScene;
	private PackedScene _libraryScene;
	
	private CardDataEditor _editorInstance;
	private CardLibrary _libraryInstance;

	private PanelType _activePanel;
	
	public override void _EnterTree()
	{
		CallDeferred(nameof(CreateCardDataEditor));
	}

	private void CreateCardDataEditor()
	{
		_editorScene = GD.Load<PackedScene>("res://addons/CardDataEditor/card_data_editor.tscn");
		_libraryScene = GD.Load<PackedScene>("res://addons/CardDataEditor/Library/card_library.tscn");
		
		_editorInstance = _editorScene.Instantiate<CardDataEditor>();
		_editorInstance.Name = "Card Data Editor";
		
		var reloadButton = new Button { Text = "Reload Plugin" };
		reloadButton.Pressed += OnReloadPressed;
		_editorInstance.GetNode("%editor_vbox_container").AddChild(reloadButton);
		
		_libraryInstance = _libraryScene.Instantiate<CardLibrary>();
		_libraryInstance.Name = "Card Library";
		
		_libraryInstance.SearchResultClicked += LoadCard;

		AddControlToDock(DockSlot.RightUl, _editorInstance);
		AddControlToDock(DockSlot.RightUl, _libraryInstance);
	}
	
	private void LoadCard(CardData card)
	{
		_editorInstance.LoadedData = card;
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
		_editorInstance.Free();
		_libraryInstance.Free();
	}
}
#endif
