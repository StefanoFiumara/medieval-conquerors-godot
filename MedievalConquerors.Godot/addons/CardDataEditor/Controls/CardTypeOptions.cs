using System;
using System.Linq;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor.Controls;

[Tool]
public partial class CardTypeOptions : OptionButton
{
	public override void _Ready()
	{
		var values = Enum.GetValues<CardType>().OrderBy(t => (int)t);

		foreach (var type in values)
		{
			AddItem(type.ToString(), (int)type);
		}
	}
}
