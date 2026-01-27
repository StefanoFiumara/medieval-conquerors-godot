using System;
using MedievalConquerors.Editors.CustomEditors.ValueEditors;
using MedievalConquerors.Engine.Actions;

namespace MedievalConquerors.Editors.Options;

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
