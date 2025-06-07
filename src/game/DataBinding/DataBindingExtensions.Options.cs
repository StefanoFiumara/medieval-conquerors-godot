using System;
using System.Linq.Expressions;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor;
using MedievalConquerors.Entities.Editor.Options;

namespace MedievalConquerors.DataBinding;

public static partial class DataBindingExtensions
{
    public static void Bind<TOwner, TProperty>(this TagSelector selector, TOwner owner, Expression<Func<TOwner, TProperty>> propertyExpression)
    {
        var prop = GetPropertyInfo(propertyExpression);
        selector.Bind(owner, prop);
    }

    public static void Bind<TOwner>(this TagSelector selector, TOwner owner, PropertyInfo prop)
    {
        RemoveBinding(selector);

        if (prop.TryGetValue(owner, out Tags currentValue))
            selector.SelectedTags = currentValue;

        var callable = Callable.From(() =>
        {
            if (owner != null)
                prop.SetValue(owner, selector.SelectedTags);
        });

        selector.Connect(TagSelector.SignalName.TagsChanged, callable);
        _activeBindings[selector] = new SignalConnection(TagSelector.SignalName.TagsChanged, callable);
    }

    public static void Bind<TOwner, TEnum>(this EnumOptions<TEnum> options, TOwner owner, Expression<Func<TOwner, TEnum>> propertyExpression)
        where TEnum : struct, Enum
    {
        var prop = GetPropertyInfo(propertyExpression);
        options.Bind(owner, prop);
    }

    public static void Bind<TOwner, TEnum>(this EnumOptions<TEnum> options, TOwner owner, PropertyInfo prop)
        where TEnum : struct, Enum
    {
        RemoveBinding(options);

        if (prop.TryGetValue(owner, out TEnum currentValue))
            options.SelectedOption = currentValue;

        var callable = Callable.From(() =>
        {
            if (owner != null)
                prop.SetValue(owner, options.SelectedOption);
        });

        options.Connect(OptionButton.SignalName.ItemSelected, callable);
        _activeBindings[options] = new SignalConnection(OptionButton.SignalName.ItemSelected, callable);
    }

}
