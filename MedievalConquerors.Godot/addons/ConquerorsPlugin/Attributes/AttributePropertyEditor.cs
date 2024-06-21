using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.ConquerorsPlugin.Attributes;

[Tool]
public partial class AttributePropertyEditor : HBoxContainer
{
	[Export] private Label _titleLabel;
	[Export] private SpinBox _intEditor;
	[Export] private LineEdit _strEditor;
	
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
		
		else if (prop.PropertyType == typeof(float))
		{
			_strEditor.Hide();
			_intEditor.Value = (float) (prop.GetValue(attr) ?? 0);
			_intEditor.ValueChanged += v =>
			{
				prop.SetValue(attr, (float)v);
			};
		}
	}
}
