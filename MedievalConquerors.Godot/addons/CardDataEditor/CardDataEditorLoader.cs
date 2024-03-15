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
		CallDeferred(nameof(CreateCardDataEditor));
	}

	private void CreateCardDataEditor()
	{
		_editorScene = GD.Load<PackedScene>("res://addons/CardDataEditor/CardDataEditor.tscn");
		_editorInstance = _editorScene.Instantiate<Control>();
		_editorInstance.Name = "Card Data Editor";
		
		var reloadButton = new Button { Text = "Reload Plugin" };
		reloadButton.Pressed += OnReloadPressed;
		_editorInstance.GetNode("%editor_vbox_container").AddChild(reloadButton);
		
		AddControlToDock(DockSlot.RightUl, _editorInstance);
	}

	private void OnReloadPressed()
	{
		_editorInstance?.QueueFree();
		CallDeferred(nameof(CreateCardDataEditor));
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(_editorInstance);
		_editorInstance.Free();
	}
}
#endif
