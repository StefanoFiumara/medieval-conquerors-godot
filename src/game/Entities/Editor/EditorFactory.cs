using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor;

public static class EditorFactory
{
	private static readonly Dictionary<Type, Func<IValueEditor>> _valueEditorRegistry = [];

	static EditorFactory()
	{
		// TODO: Object editors?
		// Value editors
		RegisterValueEditor(typeof(string), typeof(StringEditor));
		RegisterValueEditor(typeof(int), typeof(IntEditor));
		RegisterValueEditor(typeof(float), typeof(FloatEditor));
		RegisterValueEditor(typeof(CardType), typeof(CardTypeOptions));
		RegisterValueEditor(typeof(ResourceType), typeof(ResourceOptions));
		RegisterValueEditor(typeof(Tags), () => new TagSelector { Columns = 2 });
	}

	public static IValueEditor CreateValueEditor(Type type)
	{
		if (!IsRegistered(type))
		{
			GD.PrintErr($"Editor for type {type.Name} is not registered.");
			return null;
		}

		var editor = _valueEditorRegistry[type]();
		return editor;
	}

	public static IObjectEditor CreateCustomEditor(Type type)
	{
		// TODO: Register custom editors
		return null;
	}

	/*
	 * IDEA: Custom Object Editor Registry
	 *		- Generic Object Editor is parent object
	 *		- Register custom IObjectEditor<T> with a scene UID
	 *		- In Generic object editor creation, check if we have a registered editor for type T
	 *		- If so, instantiate that scene instead of using properties container.
	 */


	private static bool IsRegistered(Type type) => _valueEditorRegistry.ContainsKey(type);

	private static void RegisterValueEditor(Type propertyType, Type editorType)
	{
		if (!typeof(IValueEditor).IsAssignableFrom(editorType))
			throw new ArgumentException($"{nameof(EditorFactory)} -- Editor type must implement IValueEditor: {editorType}");

		_valueEditorRegistry[propertyType] = () => (IValueEditor) Activator.CreateInstance(editorType);
	}

	private static void RegisterValueEditor(Type propertyType, Func<IValueEditor> editorFactory)
	{
		_valueEditorRegistry[propertyType] = editorFactory;
	}
}
