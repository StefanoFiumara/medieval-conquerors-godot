using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor;

public static class ValueEditorFactory
{
	private static readonly Dictionary<Type, Type> _editorRegistry = new();

	static ValueEditorFactory()
	{
		// Register editors
		Register(typeof(string), typeof(StringValueEditor));
		Register(typeof(int), typeof(IntValueEditor));
		Register(typeof(Tags), typeof(TagSelector));
		Register(typeof(CardType), typeof(CardTypeOptions));
		Register(typeof(ResourceType), typeof(ResourceOptions));
		// Register(typeof(float), typeof(FloatPropertyEditor));
		// Register(typeof(Tags), typeof(TagsPropertyEditor));
		// Register(typeof(ResourceType), typeof(ResourceTypePropertyEditor));
	}

	private static void Register(Type propertyType, Type editorType)
	{
		if (!typeof(IValueEditor).IsAssignableFrom(editorType))
			throw new ArgumentException($"Editor type must implement IValueEditor: {editorType}");

		_editorRegistry[propertyType] = editorType;
	}

	public static IValueEditor CreateEditor<TOwner>(TOwner owner, PropertyInfo prop)
	{
		if (_editorRegistry.TryGetValue(prop.PropertyType, out var editorType))
		{
			var editor = (IValueEditor)Activator.CreateInstance(editorType);
			editor!.Load(owner, prop);
			return editor;
		}

		GD.PrintErr($"No editor registered for type: {prop.PropertyType}");
		return null;
	}
}
