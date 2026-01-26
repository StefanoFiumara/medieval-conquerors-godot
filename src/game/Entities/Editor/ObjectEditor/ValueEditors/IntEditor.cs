using System;
using Godot;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public partial class IntEditor : CenterContainer, IValueEditor
{
    private SpinBox _editor;

    public override void _Ready()
    {
        _editor = new SpinBox {
            Value = 0,
            Step = 1,
            CustomArrowStep = 1
        };

        AddChild(_editor);
    }

    public object GetValue() => (int) _editor.Value;
    public void SetValue(object value)
    {
        if (value is int or double)
            _editor.Value = Convert.ToDouble(value);
    }

    public Control GetControl() => this;

    public void Enable() => _editor.Editable = true;
    public void Disable() => _editor.Editable = false;
}
