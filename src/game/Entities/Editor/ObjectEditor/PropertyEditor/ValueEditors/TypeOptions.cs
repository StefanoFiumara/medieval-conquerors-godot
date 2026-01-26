using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MedievalConquerors.Entities.Editor.Options;

/// <summary>
/// An Option Button populated with the type names of the given type, as well as all its inheritors.
/// </summary>
public abstract partial class TypeOptions<T> : OptionButton, IValueEditor
{
    private OrderedDictionary<string, Type> _typeMap;

    protected abstract bool IsValid(Type t);

    public Type SelectedType
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
        var options = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => t.IsAssignableTo(typeof(T)))
            .Where(IsValid)
            .OrderBy(t => t.Name);

        _typeMap = new OrderedDictionary<string, Type>
        {
            { "None", null }
        };

        foreach (var option in options)
            _typeMap.Add(option.Name, option);

        Clear();
        foreach (var attr in _typeMap.Keys)
            AddItem(attr);

        AllowReselect = false;
        Select(0);
    }

    public Control GetControl() => this;
    public object GetValue() => SelectedType;
    public void SetValue(object value)
    {
        if (value is Type t)
            SelectedType = t;
    }

    public void Enable() => Disabled = false;
    public void Disable() => Disabled = true;
}
