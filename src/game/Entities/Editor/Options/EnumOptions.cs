using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MedievalConquerors.Entities.Editor.Options;

public partial class EnumOptions<T> : OptionButton, IValueEditor
    where T : struct, Enum
{
    private readonly List<T> _options = Enum.GetValues<T>().OrderBy(t => Convert.ToInt32(t)).ToList();

    public T SelectedOption
    {
        get => Enum.Parse<T>(GetItemText(GetSelectedId()));
        set
        {
            if (GetItemCount() > 0)
                Select(_options.IndexOf(value));
        }
    }

    public override void _Ready()
    {
        Clear();

        foreach (var type in _options)
            AddItem(type.ToString(), Convert.ToInt32(type));
    }

    public Control GetControl() => this;
    public object GetValue() => SelectedOption;
    public void SetValue(object value)
    {
        if (value is T type)
            SelectedOption = type;
    }

    public void Enable() => Disabled = false;
    public void Disable() => Disabled = true;
}
