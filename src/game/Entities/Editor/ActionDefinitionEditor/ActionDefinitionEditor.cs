using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor;

public partial class ActionDefinitionEditor : PanelContainer
{
	private GameActionOptions _actionOptions;
	private PackedScene _paramEditorScene;

	public override void _Ready()
	{
		_paramEditorScene = GD.Load<PackedScene>("uid://bhmiifttu4eln");
		_actionOptions = GetNode<GameActionOptions>("%action_options");

		// TODO: when selected action changes, redraw parameters, derive parameters from action type
		// TODO: Do we need a separate scene? can we just do it in code?
	}

	public ActionDefinition CreateActionDefinition()
	{
		return new ActionDefinition
		{
			ActionType = _actionOptions.SelectedOption.FullName,
			Data = "" // TODO: serialize action parameters
		};
	}
}
