using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Editors.CustomEditors.AbilityEditor;

public interface ITitleOverride
{
	string Title { get; }
}

public partial class ActionDefinitionEditor : PanelContainer, IObjectEditor<ActionDefinition>, ITitleOverride
{
	private static readonly PackedScene _propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");

	private Type _actionType;
	private readonly Dictionary<string, PropertyEditor> _editors = [];

	public string Title { get; private set; }

	public void Load(string title, ActionDefinition source, bool allowDelete = false)
	{
		_actionType = Type.GetType(source.ActionType);
		var parameters = GetParameters(_actionType);
		Title = $"{_actionType?.Name.PrettyPrint()}";

		var margin = new MarginContainer();
		AddChild(margin);

		var editorsGrid = new GridContainer { Columns = 4 };
		margin.AddChild(editorsGrid);

		foreach (var (name, type) in parameters)
		{
			var editor = _propertyEditor.Instantiate<PropertyEditor>();
			editorsGrid.AddChild(editor);
			editor.Load(type, name, source.GetData(name, type));
			_editors.Add(name, editor);
		}
	}

	private static Dictionary<string, Type> GetParameters(Type actionType)
	{
		if (!actionType.IsAssignableTo(typeof(IAbilityLoader)))
			throw new ArgumentException($"actionType {actionType.Name} is not assignable to {nameof(IAbilityLoader)}");

		var result = actionType.GetMethod(nameof(IAbilityLoader.GetParameters))?.Invoke(null, null) as Dictionary<string, Type>;
		return result ?? [];
	}

	public ActionDefinition Create()
	{
		return new ActionDefinition
		{
			ActionType = _actionType.FullName,
			Data = string.Join(",", _editors.Select(((kvp) => $"{kvp.Key}={kvp.Value.GetValue()}")))
		};
	}

	public void Enable()
	{
		foreach (var (_, editor) in _editors)
			editor.Enable();
	}

	public void Disable()
	{
		foreach (var (_, editor) in _editors)
			editor.Disable();
	}

	public Control GetControl() => this;

}
