using Godot;

namespace MedievalConquerors.Editors.CustomEditors.ValueEditors;

public partial class StringEditor : CenterContainer, IValueEditor
{
    private LineEdit _editor;

    public override void _Ready()
    {
        _editor = new LineEdit();
        AddChild(_editor);
    }

    public object GetValue() => _editor.Text;
    public void SetValue(object value)
    {
        if (value is string str)
        {
            _editor.Text = str;
        }
    }

    public Control GetControl() => this;

    public void Enable() => _editor.Editable = true;
    public void Disable() => _editor.Editable = false;
}
