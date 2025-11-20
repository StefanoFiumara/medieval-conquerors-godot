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

	public static IValueEditor CreateValueEditor(Type type)
	{
		if (!IsRegistered(type))
		{
			GD.PrintErr($"Editor for type {type.Name} is not registered.");
			return null;
		}

		var editor = _editorRegistry[type]();
		return editor;
	}

	static EditorFactory()
	{
		// Register editors
		RegisterValueEditor(typeof(string), typeof(StringEditor));
		RegisterValueEditor(typeof(int), typeof(IntEditor));
		RegisterValueEditor(typeof(float), typeof(FloatEditor));
		RegisterValueEditor(typeof(CardType), typeof(CardTypeOptions));
		RegisterValueEditor(typeof(ResourceType), typeof(ResourceOptions));
		RegisterValueEditor(typeof(Tags), () =>
		{
			var selector = Activator.CreateInstance<TagSelector>();
			selector.Columns = 2;
			return selector;
		});
	}

	private static bool IsRegistered(Type type) => _editorRegistry.ContainsKey(type);

	private static void RegisterValueEditor(Type propertyType, Type editorType)
	{
		if (!typeof(IValueEditor).IsAssignableFrom(editorType))
			throw new ArgumentException($"{nameof(EditorFactory)} -- Editor type must implement IValueEditor: {editorType}");

		_editorRegistry[propertyType] = () => (IValueEditor) Activator.CreateInstance(editorType);
	}

	private static void RegisterValueEditor<TEditor>(Type propertyType, string sceneUid) where TEditor : class, IValueEditor
	{
		_editorRegistry[propertyType] = () =>
		{
			var editorScene = GD.Load<PackedScene>(sceneUid);
			return editorScene.Instantiate<TEditor>();
		};
	}

	private static void RegisterValueEditor(Type propertyType, Func<IValueEditor> editorFactory)
	{
		_editorRegistry[propertyType] = editorFactory;
	}
}
