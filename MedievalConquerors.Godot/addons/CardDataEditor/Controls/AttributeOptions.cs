using System;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor.Controls;

[Tool]
public partial class AttributeOptions : OptionButton
{
	// TODO: Property to grab an instance of the added attribute (?)
	public override void _Ready()
	{
		var values = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
			.Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(ICardAttribute)))
			.Select(t => t.Name);
		
		Clear();
		AddItem("None");
		foreach (var attr in values)
		{
			AddItem(attr);
		}
	}
}
