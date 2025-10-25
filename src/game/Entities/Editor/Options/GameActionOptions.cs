using System;
using System.Linq;
using MedievalConquerors.Engine.Actions;

namespace MedievalConquerors.Entities.Editor.Options;

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
