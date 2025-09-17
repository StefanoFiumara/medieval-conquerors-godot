using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;

namespace MedievalConquerors.Entities.Editor.Options;

// TODO: Can this be turned into a script that allows the user to select a base interface, and generate all the options directly?
//       This would allow us to reuse this script instead of having multiple ones with the same logic, e.g. GameActionOptions vs AttributeOptions
// NOTE: We may need to standardize on using interfaces vs base classes for this implementation
public partial class GameActionOptions : OptionButton
{
	private OrderedDictionary<string, Type> _actionTypeMap;

	private bool IsValid(Type t)
	{
		return t != null && t != typeof(GameAction) && t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(GameAction));
	}

	public Type SelectedOption
	{
		get => _actionTypeMap[GetItemText(GetSelectedId())];
		set
		{
			if (!IsValid(value))
				throw new InvalidOperationException($"{value.Name} is not a valid for {nameof(GameActionOptions)}.SelectedOption");

			foreach (var (action, type) in _actionTypeMap)
			{
				if (value == type)
				{
					Select(_actionTypeMap.IndexOf(action));
					return;
				}
			}
		}
	}

	public override void _Ready()
	{
		var map = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
			.Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(GameAction)))
			.ToDictionary(t => t.Name, t => t);

		_actionTypeMap = new OrderedDictionary<string, Type>(map);

		Clear();

		foreach (var (actionName, _) in _actionTypeMap)
			AddItem(actionName);

		Select(0);
	}
}
