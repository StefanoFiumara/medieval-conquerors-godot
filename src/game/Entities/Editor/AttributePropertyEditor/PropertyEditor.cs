using System;
using System.Linq.Expressions;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.PropertyEditors;
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

	public void Load<TOwner>(TOwner owner, PropertyInfo propertyInfo)
	{
		_propertyLabel.Text = $"{propertyInfo.Name.PrettyPrint()}:";
		var editor = ValueEditorFactory.CreateEditor(owner, propertyInfo);

		if (editor != null)
			_container.AddChild(editor.GetControl());
	}
}
