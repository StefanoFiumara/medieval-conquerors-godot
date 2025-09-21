using Godot;
using MedievalConquerors.DataBinding;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Entities.Editor.Options;

namespace MedievalConquerors.Entities.Editor;

public partial class ActionDefinitionEditor : PanelContainer
{
	private GameActionOptions _actionOptions;
	private PackedScene _paramEditorScene;

	public override void _Ready()
	{
		_paramEditorScene = GD.Load<PackedScene>("uid://bhmiifttu4eln");
		_actionOptions = GetNode<GameActionOptions>("%action_options");
	}

	public void Bind(ActionDefinition actionDef)
	{
		_actionOptions.Bind(actionDef, definition => definition.ActionType);

		// TODO: Derive parameter values based on ActionType to create action parameter editors
		// TODO: Make that process reusable so we can recreate the parameters when the selection action changes
		// TODO: Binding for the action parameters may be more complex than just a bind method
	}
}
