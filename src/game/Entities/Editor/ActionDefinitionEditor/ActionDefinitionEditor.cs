using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Entities.Editor.Options;

namespace MedievalConquerors.Entities.Editor;

public partial class ActionDefinitionEditor : PanelContainer, IObjectEditor<ActionDefinition>
{
	
	private static PackedScene _paramEditorScene = GD.Load<PackedScene>("uid://bhmiifttu4eln");

	public Control GetControl() => this;

	public void Load(string title, ActionDefinition source, bool allowDelete = false)
	{
		// TODO: Determine parameter editors to create based on source.ActionType
		// TODO: How to figure it out? some kind of mapping?
		throw new System.NotImplementedException();
	}

	public ActionDefinition Create()
	{
		// TODO: Compile list of parameter editors into response
		throw new System.NotImplementedException();
	}

	public void Enable()
	{
		// TODO: Enable all child parameter editors
		throw new System.NotImplementedException();
	}

	public void Disable()
	{
		// TODO: Disable all child parameter editors
		throw new System.NotImplementedException();
	}
}
