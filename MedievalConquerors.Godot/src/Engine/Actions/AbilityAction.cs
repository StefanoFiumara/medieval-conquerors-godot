using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Engine.Actions;

public class AbilityAction(AbilityAttribute ability) : GameAction
{
    public AbilityAttribute Ability { get; } = ability;
}
