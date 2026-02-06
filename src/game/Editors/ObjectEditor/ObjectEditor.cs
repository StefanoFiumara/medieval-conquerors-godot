using System;
using System.Linq;
using Godot;
using MedievalConquerors.Editors.CustomEditors;

namespace MedievalConquerors.Editors;

public partial class ObjectEditor : PanelContainer, IObjectEditor
{
	private static readonly PackedScene _propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");

	private Button _removeButton;
	private GridContainer _propertiesContainer;
	private PanelContainer _customEditorContainer;
	private IObjectEditor _customEditor;
	private FoldableContainer _collapsibleContainer;

	public Type ObjectType { get; private set; }

	public override void _Ready()
	{
		_removeButton = GetNode<Button>("%close_button");
		_propertiesContainer = GetNode<GridContainer>("%properties_container");
		_collapsibleContainer = GetNode<FoldableContainer>("%collapsible_container");
		_customEditorContainer = GetNode<PanelContainer>("%custom_editor_container");

		_removeButton.Connect(BaseButton.SignalName.Pressed, Callable.From(QueueFree));
	}

	public object Create()
	{
		if (ObjectType == null)
		{
			GD.PrintErr($"Attempted to create Object from ObjectEditor without calling Load()");
			return null;
		}

		if (_customEditor != null)
			return _customEditor.Create();

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

		_collapsibleContainer.Title = title ?? string.Empty;
		_collapsibleContainer.Folded = true;
		_removeButton.Visible = allowDelete;

		_customEditor = EditorFactory.CreateCustomEditor(ObjectType);
		if (_customEditor != null)
		{
			_propertiesContainer.Visible = false;
			_customEditorContainer.Visible = true;
			_customEditorContainer.AddChild(_customEditor.GetControl());
			_customEditor.Load(string.Empty, source, allowDelete: false);
		}
		else
		{
			_propertiesContainer.Visible = true;
			_customEditorContainer.Visible = false;

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
	}

	public void Enable()
	{
		foreach (var editor in _propertiesContainer.GetChildren().OfType<PropertyEditor>())
			editor.Enable();

		_customEditor?.Enable();
	}

	public void Disable()
	{
		foreach (var editor in _propertiesContainer.GetChildren().OfType<PropertyEditor>())
			editor.Disable();

		_customEditor?.Disable();
	}

	public Control GetControl() => this;
}
