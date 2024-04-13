using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.addons.CardDataEditor.Attributes;

[Tool]
public partial class AttributeEditor : PanelContainer
{
	private Label _titleLabel;
	private PackedScene _propertyEditor;
	private GridContainer _propertiesContainer;

	public Button RemoveButton { get; private set; }
	
	public override void _Ready()
	{
		_propertyEditor = GD.Load<PackedScene>("res://addons/CardDataEditor/Attributes/attribute_property_editor.tscn");
		RemoveButton = GetNode<Button>("%delete_attribute_button");
		_titleLabel = GetNode<Label>("%attribute_name_label");
		_propertiesContainer = GetNode<GridContainer>("%attribute_properties_container");
	}

	public void Load(ICardAttribute attribute)
	{
		_titleLabel.Text = attribute.GetType().Name;
		
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
}
