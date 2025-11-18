using System;
using System.Reflection;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class PropertyEditor : CenterContainer
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

	public void Load(PropertyInfo prop, string title = null)
	{
		foreach (var child in _container.GetChildren())
			child.QueueFree();

		_property = prop;
		_propertyLabel.Text = $"{title ?? prop.Name.PrettyPrint()}: ";

		_valueEditor = EditorFactory.CreateEditor(prop.PropertyType);
		if (_valueEditor != null)
			_container.AddChild(_valueEditor.GetControl());
		else
			GD.PrintErr($"Could not find value editor for property type: {prop.PropertyType.Name}");
	}

	public void ApplyTo<TOwner>(TOwner owner)
	{
		_property.SetValue(owner, _valueEditor.GetValue());
	}
}
