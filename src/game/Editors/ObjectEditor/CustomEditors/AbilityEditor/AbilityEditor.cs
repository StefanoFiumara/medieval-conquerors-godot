using System;
using Godot;
using MedievalConquerors.Editors.ListEditor;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Editors.CustomEditors.AbilityEditor;

public partial class AbilityEditor : PanelContainer, IObjectEditor<AbilityAttribute>
{
	private IListEditor<ActionDefinition> _actionDefinitionsEditor;
	private Type _abilityType;

	public override void _Ready()
	{
		_actionDefinitionsEditor = GetNode<ListEditor<ActionDefinition>>("%action_definitions_editor");
	}

	public void Load(string title, AbilityAttribute source, bool allowDelete = false)
	{
		_abilityType = source.GetType();
		_actionDefinitionsEditor.Load("Actions", source.Actions, allowDelete: false);
	}

	public AbilityAttribute Create()
	{
		if (_abilityType == null)
		{
			GD.PrintErr($"Attempted to create Ability from Ability Editor without calling Load()");
			return null;
		}

		var ability = Activator.CreateInstance(_abilityType) as AbilityAttribute;
		return ability! with
		{
			Actions = _actionDefinitionsEditor.Create()
		};
	}

	public void Enable() => _actionDefinitionsEditor.Enable();
	public void Disable() => _actionDefinitionsEditor.Disable();
	public Control GetControl() => this;
}
