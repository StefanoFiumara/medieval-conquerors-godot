using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Entities.Editor.Options;

namespace MedievalConquerors.Entities.Editor;

public partial class ActionDefinitionEditor : PanelContainer, IObjectEditor<ActionDefinition>
{
	private GameActionOptions _actionOptions;
	private PackedScene _paramEditorScene;

	public override void _Ready()
	{
		// TODO: which scene is this UID pointing to?
		_paramEditorScene = GD.Load<PackedScene>("uid://bhmiifttu4eln");
		_actionOptions = GetNode<GameActionOptions>("%action_options");

		// TODO: when selected action changes, redraw parameters, derive parameters from action type
		// TODO: Do we need a separate scene? can we just do it in code?
	}

	public Control GetControl() => this;

	public void Load(string title, ActionDefinition source, bool allowDelete = false)
	{
		throw new System.NotImplementedException();
	}

	public ActionDefinition Create()
	{
		throw new System.NotImplementedException();
	}

	public void Enable()
	{
		throw new System.NotImplementedException();
	}

	public void Disable()
	{
		throw new System.NotImplementedException();
	}
}
