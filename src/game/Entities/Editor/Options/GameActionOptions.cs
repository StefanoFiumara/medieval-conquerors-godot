using System;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Entities.Editor.ValueEditors;

namespace MedievalConquerors.Entities.Editor.Options;

public partial class GameActionOptions : TypeOptions<GameAction>
{
	protected override bool IsValid(Type t)
	{
		return t != typeof(GameAction) &&
		       t.IsClass &&
		       !t.IsAbstract &&
		       t.GetInterfaces().Contains(typeof(IAbilityLoader));
	}
}
