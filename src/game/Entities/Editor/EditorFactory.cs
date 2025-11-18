using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.Options;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor;

public static class EditorFactory
{
	private static readonly Dictionary<Type, Func<IValueEditor>> _editorRegistry = [];

	static EditorFactory()
	{
		// Register editors
		Register(typeof(string), typeof(StringEditor));
		Register(typeof(int), typeof(IntEditor));
		Register(typeof(float), typeof(FloatEditor));
		Register(typeof(Tags), typeof(TagSelector));
		Register(typeof(CardType), typeof(CardTypeOptions));
		Register(typeof(ResourceType), typeof(ResourceOptions));
		// TODO: Create ValueEditor for ActionDefinition
	}

	private static bool IsRegistered(Type type) => _editorRegistry.ContainsKey(type);

	private static void Register(Type propertyType, Type editorType)
	{
		if (!typeof(IValueEditor).IsAssignableFrom(editorType))
			throw new ArgumentException($"{nameof(EditorFactory)} -- Editor type must implement IValueEditor: {editorType}");

		_editorRegistry[propertyType] = () => (IValueEditor) Activator.CreateInstance(editorType);
	}

	private static void Register<TEditor>(Type propertyType, string sceneUid) where TEditor : class, IValueEditor
	{
		_editorRegistry[propertyType] = () =>
		{
			var editorScene = GD.Load<PackedScene>(sceneUid);
			return editorScene.Instantiate<TEditor>();
		};
	}

	private static void Register(Type propertyType, Func<IValueEditor> editorFactory)
	{
		_editorRegistry[propertyType] = editorFactory;
	}

	public static IValueEditor CreateEditor(Type type)
	{
		if (!IsRegistered(type))
		{
			GD.PrintErr($"Editor for type {type.Name} is not registered.");
			return null;
		}

		var editor = _editorRegistry[type]();
		return editor;
	}
}
