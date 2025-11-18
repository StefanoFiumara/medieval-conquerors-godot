using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor.Options;

// TODO: Turn into TypeOptions to reduce code duplication
//		 Maybe we need to make ICardAttribute a class instead of an interface to support this.
public partial class AttributeOptions : OptionButton, IValueEditor
{
	private OrderedDictionary<string, Type> _attributeTypeMap;

	public Type SelectedType
	{
		get => _attributeTypeMap[GetItemText(GetSelectedId())];
		set
		{
			foreach (var (attr, type) in _attributeTypeMap)
			{
				if (value == type)
				{
					Select(_attributeTypeMap.IndexOf(attr));
					return;
				}
			}
		}
	}

	public override void _Ready()
	{
		var options = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
			.Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(ICardAttribute)))
			.OrderBy(t => t.Name);

		_attributeTypeMap = new OrderedDictionary<string, Type>
		{
			{ "None", null }
		};

		foreach (var option in options)
			_attributeTypeMap.Add(option.Name, option);

		Clear();
		foreach (var attr in _attributeTypeMap.Keys)
			AddItem(attr);

		AllowReselect = false;
		Select(0);
	}

	public Control GetControl() => this;
	public object GetValue() => SelectedType;
	public void SetValue(object value)
	{
		if (value is Type t)
			SelectedType = t;
	}

	public void Enable() => Disabled = false;
	public void Disable() => Disabled = true;
}
