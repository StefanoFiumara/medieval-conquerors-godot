using System;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.entities.editor.attribute_property_editor;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.entities.editor.attribute_editor;

public partial class AttributeEditor : PanelContainer
{
	private PackedScene _propertyEditor;

	[Export] private Label _titleLabel;
	[Export] private GridContainer _propertiesContainer;
	[Export] private Button _removeButton;

	public event Action RemovePressed;

	public override void _Ready()
	{
		// TODO: Make this an export or reference via UID
		_propertyEditor = GD.Load<PackedScene>("res://entities/editor/attribute_property_editor/attribute_property_editor.tscn");
	}

	public override void _EnterTree()
	{
		_removeButton.Pressed += OnRemovePressed;
	}

	public override void _ExitTree()
	{
		_removeButton.Pressed -= OnRemovePressed;
	}

	public void Load(ICardAttribute attribute)
	{
		_titleLabel.Text = attribute.GetType().Name.PrettyPrint().Replace("Attribute", string.Empty);

		var props = attribute.GetType().GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.ToList();

		foreach (var prop in props)
		{
			var propEditor = _propertyEditor.Instantiate<AttributePropertyEditor>();
			_propertiesContainer.AddChild(propEditor);

			propEditor.Load(attribute, prop);
		}
	}

	private void OnRemovePressed()
	{
		RemovePressed?.Invoke();
	}
}
