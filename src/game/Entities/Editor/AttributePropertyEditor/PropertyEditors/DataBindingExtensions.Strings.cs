using System;
using System.Linq.Expressions;
using System.Reflection;
using Godot;

namespace MedievalConquerors.Entities.Editor.PropertyEditors;

public static partial class DataBindingExtensions
{
    public static void Bind<TOwner, TProperty>(this LineEdit editor, TOwner owner, Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);
        editor.Bind(owner, prop);
    }

    public static void Bind<TOwner>(this LineEdit editor, TOwner owner, PropertyInfo prop)
    {
        RemoveBinding(editor);

        if (prop.TryGetValue(owner, out string currentValue))
            editor.Text = currentValue;

        var callable = Callable.From<string>(text =>
        {
            if (owner != null)
                prop.SetValue(owner, text);
        });

        editor.Connect(LineEdit.SignalName.TextChanged, callable);
        _activeBindings[editor] = new(LineEdit.SignalName.TextChanged, callable);
    }

    public static void Bind<TOwner, TProperty>(this TextEdit editor, TOwner owner, Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);
        editor.Bind(owner, prop);
    }

    public static void Bind<TOwner>(this TextEdit editor, TOwner owner, PropertyInfo prop)
    {
        RemoveBinding(editor);

        if (prop.TryGetValue(owner, out string currentValue))
            editor.Text = currentValue;

        var callable = Callable.From(() =>
        {
            if (owner != null)
                prop.SetValue(owner, editor.Text);
        });

        editor.Connect(TextEdit.SignalName.TextChanged, callable);
        _activeBindings[editor] = new(TextEdit.SignalName.TextChanged, callable);
    }
}
