using System;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Entities.Editor.Options;

namespace MedievalConquerors.Entities.Editor.ValueEditors;

public partial class GameActionOptions : TypeOptions<GameAction>
{
	protected override bool IsValid(Type t)
	{
		return t != null &&
			   t != typeof(GameAction) &&
			   t.IsClass &&
			   !t.IsAbstract &&
			   t.IsAssignableTo(typeof(GameAction)) &&
			   t.GetInterfaces().Contains(typeof(IAbilityLoader));
	}
}
