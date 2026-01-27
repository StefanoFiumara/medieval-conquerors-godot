using System;
using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Entities.Editor;

public interface IEditor
{
    Control GetControl();
    void Enable();
    void Disable();
}

public interface IValueEditor : IEditor
{
    object GetValue();
    void SetValue(object value);
}

public interface IObjectEditor : IEditor
{
    Type ObjectType { get; }
    void Load<T>(string title, T source, bool allowDelete = false) where T : class;
    object Create();
}

public interface IObjectEditor<T> : IObjectEditor where T : class
{
    Type IObjectEditor.ObjectType => typeof(T);
    object IObjectEditor.Create() => Create();

    void IObjectEditor.Load<TSource>(string title, TSource source, bool allowDelete) => Load(title, source as T, allowDelete);
    void Load(string title, T source, bool allowDelete = false);
    new T Create();
}

public interface IListEditor<T> : IObjectEditor<List<T>>
{
    void Reset();
}
