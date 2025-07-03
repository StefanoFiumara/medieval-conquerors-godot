using System;
using System.Linq;
using System.Reflection;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;
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
public partial class ObjectEditor : PanelContainer, IPropertyEditor, IRootEditor
{
	private PackedScene _propertyEditor;
	private Label _titleLabel;
	private GridContainer _propertiesContainer;

	private object _target;
	private string _title;
	private Func<PropertyInfo, bool> _propertyFilter;

	public override void _Ready()
	{
		_propertyEditor = GD.Load<PackedScene>("uid://bti603u6u2oh");
		_titleLabel = GetNode<Label>("%name_label");
		_propertiesContainer = GetNode<GridContainer>("%properties_container");
	}

	public void Load(object target, string customTitle = null, Func<PropertyInfo, bool> propertyFilter = null)
	{
		_target = target;
		_title = customTitle;
		_propertyFilter = propertyFilter;

		CallDeferred(MethodName.DeferredLoad);
	}

	public void Load<TOwner>(TOwner owner, PropertyInfo prop)
	{
		var value = prop.GetValue(owner);
		Load(value);
	}

	private void DeferredLoad()
	{
		_titleLabel.Text = _title ?? _target.GetType().Name.PrettyPrint();

		var props = _target.GetType().GetProperties()
			.Where(p => p.GetSetMethod() != null)
			.Where(p => _propertyFilter == null || _propertyFilter(p))
			.ToList();

		foreach (var prop in props)
		{
			var propEditor = _propertyEditor.Instantiate<PropertyEditor>();
			_propertiesContainer.AddChild(propEditor);
			propEditor.Load(_target, prop);
		}
	}

	public Control GetControl() => this;
}
