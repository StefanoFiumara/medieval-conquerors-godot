using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class PropertyEditor : CenterContainer
{
	private Label _propertyLabel;
	private HBoxContainer _container;

	public override void _Ready()
	{
		_container = GetNode<HBoxContainer>("%container");
		_propertyLabel = GetNode<Label>("%property_label");
	}

	public void Load(ICardAttribute attr, PropertyInfo prop)
	{
		// foreach (var child in _container.GetChildren())
		// {
		// 	if (child != _propertyLabel)
		// 	{
		// 		RemoveChild(child);
		// 		child.QueueFree();
		// 	}
		// }

		_propertyLabel.Text = $"{prop.Name.PrettyPrint()}:";

		// TODO: Update to use data binding
		//		Perhaps this property editor class will not be needed after the data binding framework is properly defined?
		var editor = PropertyEditorFactory.CreateEditor(attr, prop);

		if (editor != null)
			_container.AddChild(editor.GetControl());
	}
}
