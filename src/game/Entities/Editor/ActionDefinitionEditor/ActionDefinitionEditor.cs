using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Entities.Editor;

public partial class ActionDefinitionEditor : PanelContainer, IObjectEditor<ActionDefinition>
{
	public Control GetControl() => this;

	public void Load(string title, ActionDefinition source, bool allowDelete = false)
	{
		var t = Type.GetType(source.ActionType);
		var parameters = GetParameters(t);

		foreach (var parameter in parameters)
		{
			// TODO: Create an editor for each parameter
			// TODO: Can we use IValueEditor here? these parameters should all be primitives
		}

	}

	private Dictionary<string, Type> GetParameters(Type actionType)
	{
		if (!actionType.IsAssignableTo(typeof(IAbilityLoader)))
			throw new ArgumentException($"actionType {actionType.Name} is not assignable to {nameof(IAbilityLoader)}");

		var result = actionType.GetMethod(nameof(IAbilityLoader.GetParameters))?.Invoke(null, null) as Dictionary<string, Type>;
		return result ?? [];
	}

	public ActionDefinition Create()
	{
		// TODO: Serialize parameter editors into comma-delimited list
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
