using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor.Controls;

[Tool]
public partial class CardTypeOptions : OptionButton
{
	private List<CardType> _options;
	public CardType SelectedCardType
	{
		get => (CardType)GetSelectedId();
		set => Select((int) value);
	}

	public override void _Ready()
	{
		_options = Enum.GetValues<CardType>().OrderBy(t => (int)t).ToList();

		foreach (var type in _options)
		{
			AddItem(type.ToString(), (int)type);
		}
	}
}
