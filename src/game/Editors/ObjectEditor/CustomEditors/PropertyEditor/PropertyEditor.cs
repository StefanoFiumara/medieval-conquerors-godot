using System;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Editors.CustomEditors;

public partial class PropertyEditor : PanelContainer
{
	private Label _propertyLabel;
	private HBoxContainer _container;
	private IValueEditor _valueEditor;
	private PropertyInfo _property;

	public override void _Ready()
	{
		_container = GetNode<HBoxContainer>("%container");
		_propertyLabel = GetNode<Label>("%property_label");
	}

	public void Load(Type propertyType, string title = "", object source = null)
	{
		if (_valueEditor != null)
		{
			_valueEditor.GetControl().QueueFree();
			_valueEditor = null;
		}

		_propertyLabel.Text = $"{title}: ";
		_valueEditor = EditorFactory.CreateValueEditor(propertyType);
		if (_valueEditor == null) return;

		_container.AddChild(_valueEditor.GetControl());
		if(source != null)
			_valueEditor.SetValue(source);
	}

	public void Load(PropertyInfo prop, string title = null)
	{
		_property = prop;
		title ??= prop.Name.PrettyPrint();
		Load(prop.PropertyType, title);
	}

	public void ApplyTo<TOwner>(TOwner owner) => _property?.SetValue(owner, _valueEditor?.GetValue());
	public void SetValue(object value) => _valueEditor?.SetValue(value);
	public object GetValue() => _valueEditor?.GetValue();

	public void Enable() => _valueEditor?.Enable();
	public void Disable() => _valueEditor?.Disable();
}
