using System;
using System.Linq;
using System.Reflection;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class ObjectEditor : PanelContainer
{
	private PackedScene _propertyEditor;
	private GridContainer _propertiesContainer;

	private string _title;
	private Label _titleLabel;

	public override void _Ready()
	{
		_propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");
		_titleLabel = GetNode<Label>("%name_label");
		_propertiesContainer = GetNode<GridContainer>("%properties_container");
	}

	// TODO: is this error prone? What if there's a type mismatch?
	public T Create<T>() where T : new()
	{
		var result = new T();
		var editors = _propertiesContainer
			.GetChildren()
			.OfType<PropertyEditor>();

		foreach (var editor in editors)
			editor.ApplyTo(result);

		return result;
	}

	public void Load(Type type)
	{
		_titleLabel.Text = _title;

		var props = type.GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.ToList();

		foreach (var prop in props)
		{
			var editor = _propertyEditor.Instantiate<PropertyEditor>();
			_propertiesContainer.AddChild(editor);
			editor.Load(prop);
		}
	}

	public Control GetControl() => this;
}
