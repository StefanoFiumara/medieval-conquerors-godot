using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Editors;

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
