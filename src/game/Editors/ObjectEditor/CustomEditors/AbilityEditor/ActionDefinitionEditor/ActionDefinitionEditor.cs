using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Editors.CustomEditors.AbilityEditor;

public partial class ActionDefinitionEditor : PanelContainer, IObjectEditor<ActionDefinition>
{
	private static readonly PackedScene _propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");

	private GameActionOptions _actionOptions;

	private GridContainer _editorsContainer;
	private readonly Dictionary<string, PropertyEditor> _editors = [];

	public override void _Ready()
	{
		_actionOptions = GetNode<GameActionOptions>("%action_options");
		_editorsContainer = GetNode<GridContainer>("%editors_container");

		_actionOptions.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnActionTypeChanged));
	}

	private void OnActionTypeChanged(long itemIndex)
	{
		Load(_actionOptions.SelectedType);
	}

	private void Load(Type actionType) => Load("", new ActionDefinition { ActionType = actionType?.FullName }, allowDelete: true);
	public void Load(string title, ActionDefinition source, bool allowDelete = false)
	{
		if (!string.IsNullOrEmpty(source.ActionType))
		{
			_actionOptions.SelectedType = Type.GetType(source.ActionType);
		}

		var parameters = _actionOptions.SelectedType != null ? GetParameters(_actionOptions.SelectedType) : [];

		_editors.Clear();
		foreach (var editor in _editorsContainer.GetChildren())
			editor.QueueFree();

		foreach (var (name, type) in parameters)
		{
			var editor = _propertyEditor.Instantiate<PropertyEditor>();
			_editorsContainer.AddChild(editor);

			// TODO: Do we need to support custom editors for other parameter values?
			IValueEditor customValueEditor = name == "CardId" ? new CardIdSelector() : null;
			editor.Load(type, name.PrettyPrint(), source.GetData(name, type), customValueEditor);
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
			ActionType = _actionOptions.SelectedType.FullName,
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
