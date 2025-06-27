using System;
using System.Collections;
using System.Reflection;
using Godot;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

/// <summary>
/// EditorFactory returns the correct Godot Control for a property: ValueEditor, ObjectEditor, or ListEditor.
/// </summary>
public static class EditorFactory
{
	public static Control CreateEditor(object owner, PropertyInfo prop)
	{
		var type = prop.PropertyType;

		if (type.IsPrimitive || type == typeof(string))
			return ValueEditorFactory.CreateEditor(owner, prop)?.GetControl();

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
		{
			var listEditorScene = GD.Load<PackedScene>("uid://your_list_editor_uid"); // TODO: Replace with actual UID
			var listEditor = listEditorScene.Instantiate<ListEditor>();
			var list = prop.GetValue(owner) as IList;
			var itemType = type.GetGenericArguments()[0];

			listEditor.Load(list, itemType, prop.Name.PrettyPrint());

			return listEditor;
		}

		var objectEditorScene = GD.Load<PackedScene>("uid://bti603u6u2oh");
		var objectEditor = objectEditorScene.Instantiate<ObjectEditor>();
		var value = prop.GetValue(owner);

		objectEditor.Load(value, prop.Name.PrettyPrint());

		return objectEditor;
	}
}
