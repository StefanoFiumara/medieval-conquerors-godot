using System;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Entities.Editor;

public partial class AttributeEditor : PanelContainer
{
	private PackedScene _propertyEditor;

	[Export] private Label _titleLabel;
	[Export] private GridContainer _propertiesContainer;
	[Export] private Button _removeButton;

	public event Action RemovePressed;

	public override void _Ready()
	{
		_propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");
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
			.Where(p => p.Name != nameof(ICardAttribute.Owner))
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
