using System.Reflection;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

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

	public void Load(PropertyInfo prop, string title = null)
	{
		if (_valueEditor != null)
		{
			_valueEditor.GetControl().QueueFree();
			_valueEditor = null;
		}

		_property = prop;
		_propertyLabel.Text = $"{title ?? prop.Name.PrettyPrint()}: ";

		_valueEditor = EditorFactory.CreateValueEditor(prop.PropertyType);
		if (_valueEditor != null)
			_container.AddChild(_valueEditor.GetControl());
	}

	public void ApplyTo<TOwner>(TOwner owner) => _property.SetValue(owner, _valueEditor?.GetValue());
	public void SetValue(object value) => _valueEditor?.SetValue(value);

	public void Enable() => _valueEditor?.Enable();
	public void Disable() => _valueEditor?.Disable();
}
