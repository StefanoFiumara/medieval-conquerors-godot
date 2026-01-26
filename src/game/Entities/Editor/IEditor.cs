using System;
using Godot;

namespace MedievalConquerors.Entities.Editor;

public interface IEditor
{
    Control GetControl();
    void Enable();
    void Disable();
}

// TODO: It may streamline a lot of things if we could merge IValueEditor and IObjectEditor into the same interface
//       That way our editor registry can create either editor type from one interface.
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
