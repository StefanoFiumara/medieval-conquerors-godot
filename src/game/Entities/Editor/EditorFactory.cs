using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Entities.Editor.ValueEditors;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor;

public static class EditorFactory
{
	public const string LIST_EDITOR_UID = "uid://cgfb6ruan5t2n";
	public const string OBJ_EDITOR_UID = "uid://bxlv4w3wwtsro";

	private static readonly Dictionary<Type, Func<IPropertyEditor>> _editorRegistry = [];

	static EditorFactory()
	{
		// Register editors
		Register(typeof(string), typeof(StringEditor));
		Register(typeof(int), typeof(IntEditor));
		Register(typeof(Tags), typeof(TagSelector));
		Register(typeof(CardType), typeof(CardTypeOptions));
		Register(typeof(ResourceType), typeof(ResourceOptions));
		// TODO: Register with factory method for Ability's Action Definition editor
		// Register(typeof(float), typeof(FloatPropertyEditor));
	}

	private static bool IsRegistered(Type type) => _editorRegistry.ContainsKey(type);

	private static void Register(Type propertyType, Type editorType)
	{
		if (!typeof(IPropertyEditor).IsAssignableFrom(editorType))
			throw new ArgumentException($"{nameof(EditorFactory)} -- Editor type must implement IValueEditor: {editorType}");

		_editorRegistry[propertyType] = () => (IPropertyEditor) Activator.CreateInstance(editorType);
	}

	private static void Register<TEditor>(Type propertyType, string sceneUid) where TEditor : class, IPropertyEditor
	{
		_editorRegistry[propertyType] = () =>
		{
			var editorScene = GD.Load<PackedScene>(sceneUid);
			return editorScene.Instantiate<TEditor>();
		};
	}

	private static void Register(Type propertyType, Func<IPropertyEditor> editorFactory)
	{
		_editorRegistry[propertyType] = editorFactory;
	}

	public static IPropertyEditor CreateEditor(object owner, PropertyInfo prop)
	{
		var type = prop.PropertyType;
		if (IsRegistered(type))
		{
			var editor = _editorRegistry[type]();
			editor.Load(owner, prop);
			return editor;
		}

		// TODO: can we register this so all the different editors simply go to the registry?
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			var listEditorScene = GD.Load<PackedScene>(LIST_EDITOR_UID);
			var listEditor = listEditorScene.Instantiate<ListEditor>();
			listEditor.Load(owner, prop);

			return listEditor;
		}

		var objectEditorScene = GD.Load<PackedScene>(OBJ_EDITOR_UID);
		var objectEditor = objectEditorScene.Instantiate<ObjectEditor>();
		var value = prop.GetValue(owner);

		objectEditor.Load(value, prop.Name.PrettyPrint());

		return objectEditor;
	}

	public static IRootEditor CreateEditor(object root, string customName = null)
	{
		var type = root.GetType();
		customName ??= type.Name.PrettyPrint();

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			var listEditorScene = GD.Load<PackedScene>(LIST_EDITOR_UID);
			var listEditor = listEditorScene.Instantiate<ListEditor>();
			listEditor.Load(root, customName);

			return listEditor;
		}

		// TODO: Support for custom root editors from the registry?
		var objectEditorScene = GD.Load<PackedScene>(OBJ_EDITOR_UID);
		var objectEditor = objectEditorScene.Instantiate<ObjectEditor>();
		objectEditor.Load(root, customName);

		return objectEditor;
	}
}
