using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor;

public static class EditorFactory
{
	private static readonly PackedScene _objectEditor = GD.Load<PackedScene>("uid://bxlv4w3wwtsro");
	private static readonly Dictionary<Type, Func<IValueEditor>> _valueEditorRegistry = [];

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

	// TODO: Combine Object Editor and Value Editors into the same registry.
	// TODO: Do we need to support registering an editor with a scene UID?
	public static IObjectEditor CreateObjectEditor(Type type)
	{
		GD.Print($"Creating Editor for {type.Name}");
		// TODO: Create registry for Object Editors
		// TODO: Add support for custom attribute editors (e.g. ability editor)
		// IDEA: can this registry be incorporated in EditorFactory?
		return _objectEditor.Instantiate<ObjectEditor>();
	}

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
