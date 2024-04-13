using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.addons.CardDataEditor.Attributes;

[Tool]
public partial class AttributePropertyEditor : HBoxContainer
{
	private Label _titleLabel;
	private SpinBox _intEditor;
	private LineEdit _strEditor;
	
	public override void _Ready()
	{
		_titleLabel = GetNode<Label>("%property_label");
		_intEditor = GetNode<SpinBox>("%property_editor_int");
		_strEditor = GetNode<LineEdit>("%property_editor_str");
	}

	public void Load(ICardAttribute attr, PropertyInfo prop)
	{
		_titleLabel.Text = $"{prop.Name}:";

		if (prop.PropertyType == typeof(string))
		{
			_intEditor.Hide();
			_strEditor.Text = (string)prop.GetValue(attr);
			_strEditor.TextChanged += txt =>
			{
				prop.SetValue(attr, txt);
			};
		}
		
		else if (prop.PropertyType == typeof(int))
		{
			_strEditor.Hide();
			_intEditor.Value = (int) (prop.GetValue(attr) ?? 0);
			_intEditor.ValueChanged += v =>
			{
				prop.SetValue(attr, (int)v);
			};
		}
	}
}
