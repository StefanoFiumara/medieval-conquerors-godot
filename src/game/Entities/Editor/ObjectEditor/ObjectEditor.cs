using System;
using System.Linq;
using Godot;

namespace MedievalConquerors.Entities.Editor;

public partial class ObjectEditor : PanelContainer, IObjectEditor
{
	private PackedScene _propertyEditor;
	private GridContainer _propertiesContainer;

	private Label _titleLabel;
	private Button _removeButton;
	public Type ObjectType { get; private set; }

	public override void _Ready()
	{
		_propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");
		_titleLabel = GetNode<Label>("%name_label");
		_removeButton = GetNode<Button>("%close_button");
		_propertiesContainer = GetNode<GridContainer>("%properties_container");

		_removeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(QueueFree));
	}

	public object Create()
	{
		if (ObjectType == null)
		{
			GD.PrintErr($"Attempted to create Object from ObjectEditor without calling Load()");
			return null;
		}

		var result = Activator.CreateInstance(ObjectType);

		var editors = _propertiesContainer
			.GetChildren()
			.OfType<PropertyEditor>();

		foreach (var editor in editors)
			editor.ApplyTo(result);

		return result;
	}

	public void Load<T>(string title, T source, bool allowDelete = false) where T : class
	{
		ObjectType = source.GetType();
		_titleLabel.Text = title ?? string.Empty;
		_removeButton.Visible = allowDelete;

		var props = ObjectType.GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.ToList();

		foreach (var prop in props)
		{
			var editor = _propertyEditor.Instantiate<PropertyEditor>();
			_propertiesContainer.AddChild(editor);
			editor.Load(prop);

			var propValue = prop.GetValue(source);
			editor.SetValue(propValue);
		}
	}

	public void Enable()
	{
		foreach (var editor in _propertiesContainer.GetChildren().OfType<PropertyEditor>())
			editor.Enable();
	}

	public void Disable()
	{
		foreach (var editor in _propertiesContainer.GetChildren().OfType<PropertyEditor>())
			editor.Disable();
	}

	public Control GetControl() => this;
}
