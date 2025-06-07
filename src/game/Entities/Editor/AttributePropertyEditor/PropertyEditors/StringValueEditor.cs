using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor.PropertyEditors;

public partial class StringValueEditor : PanelContainer, IValueEditor
{
    public void Load<TOwner>(TOwner owner, PropertyInfo prop)
    {
        var editor = new LineEdit();
        editor.Bind(owner, prop);
        AddChild(editor);
    }

    public Control GetControl() => this;
}
