using System.Reflection;
using Godot;
using MedievalConquerors.DataBinding;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public partial class TagsValueEditor : PanelContainer, IValueEditor
{
    public void Load<TOwner>(TOwner owner, PropertyInfo prop)
    {
        var tagSelector = new TagSelector();
        tagSelector.Bind(owner, prop);
        AddChild(tagSelector);
    }

    public Control GetControl() => this;
}
