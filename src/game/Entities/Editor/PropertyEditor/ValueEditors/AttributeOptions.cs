using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

// TODO: turn into TypeOptions? Maybe we need to make ICardAttribute a class instead of an interface.
public partial class AttributeOptions : OptionButton, IValueEditor
{
	private Dictionary<string, Type> _attributeTypeMap;

	public override void _Ready()
	{

		_attributeTypeMap = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
			.Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(ICardAttribute)))
			.ToDictionary(t => t.Name, t => t);

		Clear();
		AddItem("None");

		foreach (var attr in _attributeTypeMap.Keys)
			AddItem(attr);
	}

	public Type GetSelectedType()
	{
		var selected = GetItemText(GetSelectedId());
		return _attributeTypeMap.GetValueOrDefault(selected);
	}

	public ICardAttribute CreateSelected()
	{
		var selected = GetItemText(GetSelectedId());

		if (_attributeTypeMap.TryGetValue(selected, out var attributeType))
			return (ICardAttribute)Activator.CreateInstance(attributeType);

		return null;
	}

	public Control GetControl() => this;
	public object GetValue() => GetSelectedType();
}
