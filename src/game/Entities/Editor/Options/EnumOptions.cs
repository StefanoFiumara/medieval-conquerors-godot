using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor.Options;

public partial class EnumOptions<T> : OptionButton, IValueEditor
    where T : struct, Enum
{
    private readonly List<T> _options = Enum.GetValues<T>().OrderBy(t => Convert.ToInt32(t)).ToList();
    private T _selectedOption;
    public T SelectedOption
    {
        get => _selectedOption;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_selectedOption, value))
                return;

            _selectedOption = value;
            if (GetItemCount() > 0)
                Select(_options.IndexOf(value));
        }
    }

    public override void _Ready()
    {
        Clear();

        foreach (var type in _options)
            AddItem(type.ToString(), Convert.ToInt32(type));

        Select(_options.IndexOf(_selectedOption));
    }

    public Control GetControl() => this;
    public object GetValue() => SelectedOption;
}
