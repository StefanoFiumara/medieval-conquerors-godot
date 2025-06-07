using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Entities.Editor.Options;

public partial class AttributeOptions : OptionButton
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
		{
			AddItem(attr);
		}
	}

	public ICardAttribute CreateSelected()
	{
		var selected = GetItemText(GetSelectedId());
		if (_attributeTypeMap.TryGetValue(selected, out var attributeType))
		{
			return (ICardAttribute)Activator.CreateInstance(attributeType);
		}

		return null;
	}
}
