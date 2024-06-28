using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MedievalConquerors.ConquerorsPlugin.Controls;

public partial class EnumOptions<T> : OptionButton
    where T : struct, Enum
{
    private List<T> _options;
    public T SelectedOption
    {
        get => (T)Enum.ToObject(typeof(T), GetSelectedId());
        set => Select(Convert.ToInt32(value));
    }

    public override void _Ready()
    {
        _options = Enum.GetValues<T>().OrderBy(t => Convert.ToInt32(t)).ToList();

        Clear();
        foreach (var type in _options)
        {
            AddItem(type.ToString(), Convert.ToInt32(type));
        }
    }
}