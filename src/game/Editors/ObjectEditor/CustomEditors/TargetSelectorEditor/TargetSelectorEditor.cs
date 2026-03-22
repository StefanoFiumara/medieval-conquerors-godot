using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Editors.CustomEditors;

public partial class TargetSelectorEditor : PanelContainer, IObjectEditor<TargetSelectorAttribute>
{
	private static readonly PackedScene _propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");

	private TargetSelectorOptions _options;
	private HBoxContainer _paramsContainer;
	private readonly Dictionary<string, PropertyEditor> _editors = [];

	public override void _Ready()
	{
		_options = GetNode<TargetSelectorOptions>("%selector_options");
		_paramsContainer = GetNode<HBoxContainer>("%params_container");

		_options.Connect(OptionButton.SignalName.ItemSelected, Callable.From<long>(OnActionTypeChanged));
	}

	private void OnActionTypeChanged(long itemIndex) => Load(_options.SelectedType);

	private void Load(Type selectorType)
	{
		Load("", new TargetSelectorAttribute
		{
			Selector = (TargetSelector) Activator.CreateInstance(selectorType)
		});
	}

	public void Load(string title, TargetSelectorAttribute source, bool allowDelete = false)
	{
		if (source.Selector == null)
			return;

		_options.SelectedType = source.Selector.GetType();

		var props = _options.SelectedType.GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.ToList();

		_editors.Clear();
		foreach (var editor in _paramsContainer.GetChildren())
			editor.QueueFree();

		foreach (var prop in props)
		{
			var editor = _propertyEditor.Instantiate<PropertyEditor>();
			_paramsContainer.AddChild(editor);
			_editors.Add(prop.Name, editor);
			editor.Load(prop);

			var propValue = prop.GetValue(source.Selector);
			editor.SetValue(propValue);
		}
	}

	public TargetSelectorAttribute Create()
	{
		var selector = (TargetSelector) Activator.CreateInstance(_options.SelectedType);
		foreach (var editor in _editors.Values)
		{
			editor.ApplyTo(selector);
		}

		return new TargetSelectorAttribute
		{
			Selector = selector
		};
	}

	public void Enable()
	{
		_options.Enable();

		foreach (var (_, editor) in _editors)
			editor.Enable();

	}

	public void Disable()
	{
		_options.Disable();

		foreach (var (_, editor) in _editors)
			editor.Disable();
	}

	public Control GetControl() => this;
}
