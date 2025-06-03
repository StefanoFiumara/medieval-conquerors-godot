using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Entities.Editor;

public interface IValueEditor
{
    Control GetControl();
    void Initialize(ICardAttribute attribute, PropertyInfo propertyInfo);
}

public static class PropertyEditorFactory
{
    private static readonly Dictionary<Type, Type> EditorRegistry = new();

    static PropertyEditorFactory()
    {
        // TODO: Possible way to map everything automatically by using the types?
        //      Maybe re-introduce the generic parameter to the interface?
        // Register editors
        Register(typeof(string), typeof(PropertyEditors.StringValueEditor));
        Register(typeof(int), typeof(PropertyEditors.IntValueEditor));
        // Register(typeof(float), typeof(FloatPropertyEditor));
        // Register(typeof(Tags), typeof(TagsPropertyEditor));
        // Register(typeof(ResourceType), typeof(ResourceTypePropertyEditor));
    }

    private static void Register(Type propertyType, Type editorType)
    {
        if (!typeof(IValueEditor).IsAssignableFrom(editorType))
            throw new ArgumentException($"Editor type must implement IValueEditor: {editorType}");

        EditorRegistry[propertyType] = editorType;
    }

    public static IValueEditor CreateEditor(ICardAttribute attribute, PropertyInfo propertyInfo)
    {
        if (EditorRegistry.TryGetValue(propertyInfo.PropertyType, out var editorType))
        {
            var editor = (IValueEditor)Activator.CreateInstance(editorType);
            editor!.Initialize(attribute, propertyInfo);
            return editor;
        }

        GD.PrintErr($"No editor registered for type: {propertyInfo.PropertyType}");
        return null;
    }
}
