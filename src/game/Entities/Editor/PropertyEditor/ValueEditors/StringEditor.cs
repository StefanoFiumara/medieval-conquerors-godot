using Godot;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public partial class StringEditor : CenterContainer, IValueEditor
{
    private LineEdit _editor;

    public override void _Ready()
    {
        _editor = new LineEdit();
        AddChild(_editor);
    }

    public object GetValue() => _editor.Text;
    public Control GetControl() => this;
}
