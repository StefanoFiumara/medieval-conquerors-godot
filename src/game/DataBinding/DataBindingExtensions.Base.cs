using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Godot;

namespace MedievalConquerors.DataBinding;

public static partial class DataBindingExtensions
{
    private record SignalConnection(StringName Signal, Callable Callable);
    private static readonly Dictionary<Control, SignalConnection> _activeBindings = new();

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

    private static PropertyInfo GetPropertyInfo<TOwner, TProperty>(this Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression { Member: PropertyInfo propertyInfo })
            return propertyInfo;

        throw new ArgumentException("Expression must be a property access expression", nameof(propertyExpression));
    }
}
