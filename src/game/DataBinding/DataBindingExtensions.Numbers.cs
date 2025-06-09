using System;
using System.Linq.Expressions;
using System.Reflection;
using Godot;
using Range = Godot.Range;

namespace MedievalConquerors.DataBinding;

public static partial class DataBindingExtensions
{
    // TODO: Figure out how to support bindings for float vs int values
    public static void Bind<TOwner, TProperty>(this SpinBox editor, TOwner owner, Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);
        editor.Bind(owner, prop);
    }

    public static void Bind<TOwner>(this SpinBox editor, TOwner owner, PropertyInfo prop)
    {
        RemoveBinding(editor);

        editor.Rounded = true;
        editor.Step = 1;
        editor.CustomArrowStep = 1;

        if (prop.TryGetValue(owner, out int currentValue))
            editor.Value = currentValue;

        var callable = Callable.From<double>(value =>
        {
            if (owner != null)
                prop.SetValue(owner, (int)value);
        });

        editor.Connect(Range.SignalName.ValueChanged, callable);
        _activeBindings[editor] = new SignalConnection(Range.SignalName.ValueChanged, callable);
    }
}
