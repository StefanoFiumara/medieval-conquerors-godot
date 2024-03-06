#if TOOLS
using Godot;

namespace MedievalConquerors.Addons.CardDataEditor;

[Tool]
public partial class CardDataEditorLoader : EditorPlugin
{
	private PackedScene _editorScene;
	private Control _editorInstance;
	
	public override void _EnterTree()
	{
		LoadDockScene();
	}

	private void LoadDockScene()
	{
		_editorInstance?.QueueFree();
		_editorScene = GD.Load<PackedScene>("res://addons/CardDataEditor/CardDataEditor.tscn");
		_editorInstance = _editorScene.Instantiate<Control>();
		_editorInstance.Name = "Card Data Editor";
		
		var reloadButton = new Button { Text = "Reload Plugin" };
		reloadButton.Pressed += OnReloadPressed;
		_editorInstance.AddChild(reloadButton);
		
		AddControlToDock(DockSlot.RightUl, _editorInstance);
	}

	private void OnReloadPressed()
	{
		CallDeferred("LoadDockScene");
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(_editorInstance);
		_editorInstance.Free();
	}
}
#endif
