using System;
using System.Linq;
using System.Reflection;
using Godot;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

/// <summary>
/// ObjectEditor is a generic Godot editor panel for displaying and editing the properties of any object instance.
///
/// Usage:
/// - Call Load(target) to display all settable properties of the target object.
/// - Optionally provide a custom title for the editor panel.
/// - Optionally provide a propertyFilter to control which properties are shown.
///   The filter is applied after ensuring the property has a set method.
///
/// <code>
/// // Example usage:
/// editor.Load(myObject, "Custom Title", p => p.Name != "Id");
/// </code>
/// </summary>
public partial class ObjectEditor : PanelContainer
{
	private PackedScene _propertyEditor;
	private Label _titleLabel;
	private GridContainer _propertiesContainer;

	public override void _Ready()
	{
		_propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");
		_titleLabel = GetNode<Label>("%name_label");
		_propertiesContainer = GetNode<GridContainer>("%properties_container");
	}

	/// <summary>
	/// Loads the editor with the properties of the specified target object.
	/// </summary>
	/// <param name="target">The object whose properties will be displayed and edited.</param>
	/// <param name="customTitle">Optional. A custom title for the editor panel. If null, the type's name is used.</param>
	/// <param name="propertyFilter">Optional. A filter to determine which properties to display. The filter is applied after ensuring the property has a public set method.</param>
	public void Load(object target, string customTitle = null, Func<PropertyInfo, bool> propertyFilter = null)
	{
		_titleLabel.Text = customTitle ?? target.GetType().Name.PrettyPrint();

		var props = target.GetType().GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.Where(p => propertyFilter == null || propertyFilter(p))
			.ToList();

		foreach (var prop in props)
		{
			var propEditor = _propertyEditor.Instantiate<PropertyEditor>();
			_propertiesContainer.AddChild(propEditor);
			propEditor.Load(target, prop);
		}
	}
}
