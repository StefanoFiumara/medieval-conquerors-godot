using System;
using System.Linq;
using Godot;

namespace MedievalConquerors.Entities.Editor;

// TODO: Extract interface and use a registry to determine which editor type to create in Attributes editor.
//		This way we can support custom editors depending on the Attribute Type.
public interface IEditor
{
	Type ObjectType { get; }
	void Load(Type type, string title, object source = null);

	Control GetControl();
	object Create();
	void Enable();
	void Disable();
}

public partial class ObjectEditor : PanelContainer
{
	private PackedScene _propertyEditor;
	private GridContainer _propertiesContainer;

	private Label _titleLabel;
	public Type ObjectType { get; private set; }

	// TODO: Add an option to close this editor with an X button (Calls QueueFree on itself)
	//		This should allow the editor to add/remove itself from the attribute list in the Attributes Editor without additional logic needed.
	//		It would work because the Attributes Editor simply looks through its nodes to find which attributes to create.
	//
	//		The X button should default to invisible unless specifically enabled by the caller, since we may not want to grant
	//		all object editors this functionality.

	public override void _Ready()
	{
		_propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");
		_titleLabel = GetNode<Label>("%name_label");
		_propertiesContainer = GetNode<GridContainer>("%properties_container");
	}

	public object CreateObject()
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

	// TODO: Can we support this generic version of Load instead?
	// IDEA: If so, can we do the same with Create()?
	//		 Or would that require this to be IEditor<T> instead of just IEditor?
	public void Load<T>(string title, T source = null) where T : class
		=> Load(typeof(T), title, source);

	public void Load(Type type, string title, object source = null)
	{
		ObjectType = type;
		_titleLabel.Text = title ?? string.Empty;

		var props = ObjectType.GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.ToList();

		foreach (var prop in props)
		{
			var editor = _propertyEditor.Instantiate<PropertyEditor>();
			_propertiesContainer.AddChild(editor);
			editor.Load(prop);
			if (source != null)
			{
				var propValue = prop.GetValue(source);
				editor.SetValue(propValue);
			}

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
}
