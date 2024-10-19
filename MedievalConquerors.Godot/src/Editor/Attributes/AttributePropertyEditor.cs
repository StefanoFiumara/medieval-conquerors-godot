using System.Reflection;
using Godot;
using MedievalConquerors.Editor.Controls;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Editor.Attributes;

public partial class AttributePropertyEditor : HBoxContainer
{
	[Export] private Label _titleLabel;
	[Export] private SpinBox _intEditor;
	[Export] private LineEdit _strEditor;
	[Export] private TagOptions _tagsEditor;
	[Export] private ResourceOptions _resourceEditor;

	public override void _Ready()
	{
		_intEditor.Hide();
		_strEditor.Hide();
		_tagsEditor.Hide();
		_resourceEditor.Hide();
	}

	public void Load(ICardAttribute attr, PropertyInfo prop)
	{
		_titleLabel.Text = $"{prop.Name.PrettyPrint()}:";

		if (prop.PropertyType == typeof(string))
		{
			_strEditor.Show();
			_strEditor.Text = (string)prop.GetValue(attr);
			_strEditor.TextChanged += txt => prop.SetValue(attr, txt);
		}
		
		else if (prop.PropertyType == typeof(int))
		{
			_intEditor.Show();
			_intEditor.Value = (int) (prop.GetValue(attr) ?? 0);
			_intEditor.Step = 1;
			_intEditor.CustomArrowStep = 1;
			_intEditor.ValueChanged += v => prop.SetValue(attr, (int)v);
		}
		
		else if (prop.PropertyType == typeof(float))
		{
			_intEditor.Show();
			_intEditor.Value = (float) (prop.GetValue(attr) ?? 0);
			_intEditor.Step = 0.1;
			_intEditor.CustomArrowStep = 0.1;
			_intEditor.ValueChanged += v => prop.SetValue(attr, (float)v);
		}
		
		else if (prop.PropertyType == typeof(Tags))
		{
			_tagsEditor.Show();
			_tagsEditor.SelectedTags = (Tags) (prop.GetValue(attr) ?? Tags.None);
			_tagsEditor.TagsChanged += () => prop.SetValue(attr, _tagsEditor.SelectedTags);
		}
		
		else if (prop.PropertyType == typeof(ResourceType))
		{
			_resourceEditor.Show();
			_resourceEditor.SelectedOption = (ResourceType)(prop.GetValue(attr) ?? ResourceType.None);
			_resourceEditor.ItemSelected += i => prop.SetValue(attr, _resourceEditor.SelectedOption);
		}
	}
}
