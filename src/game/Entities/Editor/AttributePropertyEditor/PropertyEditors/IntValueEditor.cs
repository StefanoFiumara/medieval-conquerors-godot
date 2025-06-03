using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Entities.Editor.PropertyEditors;

public partial class IntValueEditor : PanelContainer, IValueEditor
{
    private SpinBox _editor;
    private ICardAttribute _attribute;
    private PropertyInfo _property;

    public void Initialize(ICardAttribute attribute, PropertyInfo propertyInfo)
    {
        _attribute = attribute;
        _property = propertyInfo;

        _editor = new SpinBox {
            Value = (int)(propertyInfo.GetValue(attribute) ?? 0),
            Step = 1,
            CustomArrowStep = 1
        };
        _editor.ValueChanged += OnValueChanged;

        AddChild(_editor);
    }

    private void OnValueChanged(double value)
    {
        _property.SetValue(_attribute, (int)value);
    }

    public override void _ExitTree()
    {
        if(_editor != null)
            _editor.ValueChanged -= OnValueChanged;
    }

    public Control GetControl() => this;
}
