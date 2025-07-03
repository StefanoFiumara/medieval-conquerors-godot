using System;
using System.Reflection;
using Godot;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public interface IEditor
{
    Control GetControl();
}

public interface IRootEditor : IEditor
{
    /// <summary>
    /// Loads the editor with the properties of the specified target object.
    /// </summary>
    /// <param name="target">The object whose properties will be displayed and edited.</param>
    /// <param name="title">Optional. A custom title for the editor panel. If null, the type's name is used.</param>
    /// <param name="propertyFilter">Optional. A filter to determine which properties to display. The filter is applied after ensuring the property has a public set method.</param>
    void Load(object target, string title = null, Func<PropertyInfo, bool> propertyFilter = null);
}

public interface IPropertyEditor : IEditor
{
    void Load<TOwner>(TOwner owner, PropertyInfo prop);
}
