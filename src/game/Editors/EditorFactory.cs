using System;
using System.Collections.Generic;
using Godot;
using MedievalConquerors.Editors.CustomEditors.AbilityEditor;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Editors.Options;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Editors;

public static class EditorFactory
{
	private static readonly Dictionary<Type, Func<IValueEditor>> _valueEditorRegistry = [];
	private static readonly Dictionary<Type, Func<IObjectEditor>> _customEditorRegistry = [];

	static EditorFactory()
	{
		RegisterCustomEditor(typeof(OnCardPlayedAbility), "uid://by3pryhhju02i");
		RegisterCustomEditor(typeof(OnCardActivatedAbility), "uid://by3pryhhju02i");
		RegisterCustomEditor(typeof(ActionDefinition), typeof(ActionDefinitionEditor));

		// Value editors
		RegisterValueEditor(typeof(string), typeof(StringEditor));
		RegisterValueEditor(typeof(int), typeof(IntEditor));
		RegisterValueEditor(typeof(float), typeof(FloatEditor));
		// Enums
		RegisterValueEditor(typeof(CardType), typeof(CardTypeOptions));
		RegisterValueEditor(typeof(ResourceType), typeof(ResourceOptions));
		RegisterValueEditor(typeof(PlayerTarget), typeof(PlayerTargetOptions));
		RegisterValueEditor(typeof(Zone), typeof(ZoneOptions));
		RegisterValueEditor(typeof(Tags), () => new TagSelector { Columns = 2 });
	}

	public static IValueEditor CreateValueEditor(Type type)
	{
		if (!IsValueEditorRegistered(type))
		{
			GD.PrintErr($"Editor for type {type.Name} is not registered.");
			return null;
		}

		var editor = _valueEditorRegistry[type]();
		return editor;
	}

	public static IObjectEditor CreateCustomEditor(Type type)
	{
		if (!IsCustomEditorRegistered(type))
			return null;

		var editor = _customEditorRegistry[type]();
		return editor;
	}

	private static bool IsValueEditorRegistered(Type type) => _valueEditorRegistry.ContainsKey(type);
	private static bool IsCustomEditorRegistered(Type type) => _customEditorRegistry.ContainsKey(type);

	private static void RegisterCustomEditor(Type propertyType, string sceneUid)
	{
		_customEditorRegistry[propertyType] = () =>
		{
			var scene = GD.Load<PackedScene>(sceneUid);
			return scene.Instantiate<IObjectEditor>();
		};
	}

	private static void RegisterCustomEditor(Type propertyType, Type editorType)
	{
		if (!typeof(IObjectEditor).IsAssignableFrom(editorType))
			throw new ArgumentException($"{nameof(EditorFactory)} -- Editor type must implement IObjectEditor: {editorType}");

		_customEditorRegistry[propertyType] = () => (IObjectEditor) Activator.CreateInstance(editorType);
	}

	private static void RegisterCustomEditor(Type propertyType, Func<IObjectEditor> editorFactory)
	{
		_customEditorRegistry[propertyType] = editorFactory;
	}

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
