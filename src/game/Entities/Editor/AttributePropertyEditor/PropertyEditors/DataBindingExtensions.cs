using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Godot;
using Range = Godot.Range;

namespace MedievalConquerors.Entities.Editor.PropertyEditors;

public static class DataBindingExtensions
{
    private record SignalConnection(StringName Signal, Callable Callable);
    private static readonly Dictionary<Control, SignalConnection> _activeBindings = new();

    public static void Bind<TOwner>(this SpinBox editor, TOwner owner, PropertyInfo prop)
    {
        RemoveBinding(editor);

        if (prop.PropertyType == typeof(int))
        {
            editor.Rounded = true;
            editor.Step = 1;
            editor.CustomArrowStep = 1;
        }

        if (prop.TryGetValue(owner, out int currentValue))
            editor.Value = currentValue;

        var callable = Callable.From<double>(value =>
        {
            if (owner != null)
                prop.SetValue(owner, (int)value);
        });

        editor.Connect(Range.SignalName.ValueChanged, callable);
        _activeBindings[editor] = new(Range.SignalName.ValueChanged, callable);
    }

    public static void Bind<TOwner, TProperty>(this SpinBox editor, TOwner owner, Expression<Func<TOwner, TProperty>> propertyExpression)
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

    public static void Bind<TOwner, TProperty>(this LineEdit editor, TOwner owner, Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);
        editor.Bind(owner, prop);
    }

    public static void Bind<TOwner>(this TextEdit editor, TOwner owner, PropertyInfo prop)
    {
        RemoveBinding(editor);

        if (prop.TryGetValue(owner, out string currentValue))
            editor.Text = currentValue;

        // Connect to the text_changed signal
        var callable = Callable.From(() =>
        {
            if (owner != null)
                prop.SetValue(owner, editor.Text);
        });

        editor.Connect(TextEdit.SignalName.TextChanged, callable);
        _activeBindings[editor] = new(TextEdit.SignalName.TextChanged, callable);
    }

    public static void Bind<TOwner, TProperty>(this TextEdit editor, TOwner owner, Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);
        editor.Bind(owner, prop);
    }

    private static void RemoveBinding(Control control)
    {
        if (!_activeBindings.TryGetValue(control, out var connection)) return;

        if (control.IsConnected(connection.Signal, connection.Callable))
            control.Disconnect(connection.Signal, connection.Callable);

        _activeBindings.Remove(control);
    }

    private static bool TryGetValue<T>(this PropertyInfo prop, object owner, out T value)
    {
        if (owner != null && prop.GetValue(owner) is T val)
        {
            value = val;
            return true;
        }

        value = default;
        return false;
    }

    public static PropertyInfo GetPropertyInfo<TOwner, TProperty>(this Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression { Member: PropertyInfo propertyInfo })
            return propertyInfo;

        throw new ArgumentException("Expression must be a property access expression", nameof(propertyExpression));
    }
}
