using System.Reflection;
using Godot;
using MedievalConquerors.DataBinding;

namespace MedievalConquerors.Entities.Editor.PropertyEditors;

public partial class IntValueEditor : PanelContainer, IValueEditor
{
    public void Load<TOwner>(TOwner owner, PropertyInfo prop)
    {
        var editor = new SpinBox {
            Value = (int)(prop.GetValue(owner) ?? 0),
            Step = 1,
            CustomArrowStep = 1
        };

        editor.Bind(owner, prop);
        AddChild(editor);
    }

    public Control GetControl() => this;
}
