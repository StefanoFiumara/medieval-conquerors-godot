using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor.PropertyEditors;

public partial class StringValueEditor : PanelContainer, IValueEditor
{
    private ICardAttribute _attribute;
    private PropertyInfo _property;
    private LineEdit _editor;

    public void Initialize(ICardAttribute attribute, PropertyInfo propertyInfo)
    {
        _attribute = attribute;
        _property = propertyInfo;

        _editor = new LineEdit { Text = (string)propertyInfo.GetValue(attribute) ?? string.Empty };
        _editor.TextChanged += OnTextChanged;

        AddChild(_editor);
    }

    private void OnTextChanged(string text)
    {
        _property.SetValue(_attribute, text);
    }

    public override void _ExitTree()
    {
        if(_editor != null)
            _editor.TextChanged -= OnTextChanged;
    }

    public Control GetControl() => this;
}
