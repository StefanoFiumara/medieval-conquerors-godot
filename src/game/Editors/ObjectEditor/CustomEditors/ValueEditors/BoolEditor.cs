using Godot;

namespace MedievalConquerors.Editors.CustomEditors.ValueEditors;

public partial class BoolEditor : CheckBox, IValueEditor
{
    public Control GetControl() => this;

    public void Enable() => Disabled = false;
    public void Disable() => Disabled = true;

    public object GetValue() => ButtonPressed;

    public void SetValue(object value)
    {
        if (value is bool pressed)
            ButtonPressed = pressed;
    }
}