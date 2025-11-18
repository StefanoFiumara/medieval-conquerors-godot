using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor.Options;

/// <summary>
/// An Option Button populated with the type names of the given type, as well as all its inheritors.
/// </summary>
public abstract partial class TypeOptions<T> : OptionButton, IValueEditor
{
    private OrderedDictionary<string, Type> _typeMap;

    protected abstract bool IsValid(Type t);

    public Type SelectedOption
    {
        get => _typeMap[GetItemText(GetSelectedId())];
        set
        {
            foreach (var (action, type) in _typeMap)
            {
                if (value == type)
                {
                    Select(_typeMap.IndexOf(action));
                    return;
                }
            }
        }
    }

    public override void _Ready()
    {
        Clear();

        var options = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => t.IsAssignableTo(typeof(T)))
            .Where(IsValid)
            .OrderBy(t => t.Name);

        _typeMap = new OrderedDictionary<string, Type>();

        foreach (var option in options)
            _typeMap.Add(option.Name, option);

        AllowReselect = false;
        Select(0);
    }

    public Control GetControl() => this;
    public object GetValue() => SelectedOption;
}
