using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor;

public static class ValueEditorFactory
{
    private static readonly Dictionary<Type, Type> _editorRegistry = new();

    static ValueEditorFactory()
    {
        // TODO: Possible way to map everything automatically by using the types?
        //      Maybe re-introduce the generic parameter to the interface?
        // Register editors
        Register(typeof(string), typeof(StringValueEditor));
        Register(typeof(int), typeof(IntValueEditor));
        Register(typeof(Tags), typeof(TagsValueEditor));
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
