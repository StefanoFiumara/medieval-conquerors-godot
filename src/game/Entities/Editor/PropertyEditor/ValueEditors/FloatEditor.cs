using Godot;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public partial class FloatEditor : CenterContainer, IValueEditor
{
    private SpinBox _editor;

    public override void _Ready()
    {
        _editor = new SpinBox {
            Value = 0,
            Step = 0.1,
            CustomArrowStep = 0.1
        };

        AddChild(_editor);
    }

    public object GetValue() => _editor.Value;
    public Control GetControl() => this;
}