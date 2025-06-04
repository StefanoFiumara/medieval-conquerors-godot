using System;
using System.Linq.Expressions;
using System.Reflection;
using Godot;

namespace MedievalConquerors.Entities.Editor.PropertyEditors;

public static class DataBindingExtensions
{
    public static void Bind<TProperty>(this SpinBox editor, object owner, Expression<Func<TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);

        if (prop.PropertyType == typeof(int))
        {
            editor.Rounded = true;
            editor.Step = 1;
            editor.CustomArrowStep = 1;
        }

        // Set initial value from property
        if (prop.GetValue(owner) is int currentValue)
            editor.Value = currentValue;

        editor.TreeEntered += () => editor.ValueChanged += OnValueChanged;
        editor.TreeExiting += () => editor.ValueChanged -= OnValueChanged;
        return;

        void OnValueChanged(double value)
        {
            if(owner != null)
                prop.SetValue(owner, (int) value);
        }
    }

    public static void Bind<TProperty>(this LineEdit editor, object owner, Expression<Func<TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);

        if (prop.GetValue(owner) is string currentValue)
        {
            editor.Text = currentValue;
        }

        editor.TreeEntered += () => editor.TextChanged += OnTextChanged;
        editor.TreeExiting += () => editor.TextChanged -= OnTextChanged;
        return;

        void OnTextChanged(string text)
        {
            if(owner != null)
                prop.SetValue(owner, text);
        }
    }

    public static void Bind<TProperty>(this TextEdit editor, object owner, Expression<Func<TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);

        if (prop.GetValue(owner) is string currentValue)
        {
            editor.Text = currentValue;
        }

        editor.TreeEntered += () => editor.TextChanged += OnTextChanged;
        editor.TreeExiting += () => editor.TextChanged -= OnTextChanged;
        return;

        void OnTextChanged()
        {
            if(owner != null)
                prop.SetValue(owner, editor.Text);
        }
    }

    private static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression { Member: PropertyInfo propertyInfo })
            return propertyInfo;

        throw new ArgumentException("Expression must be a property access expression", nameof(propertyExpression));
    }
}
