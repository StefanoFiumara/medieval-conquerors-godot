using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Engine.Actions;

public class AbilityAction(AbilityAttribute ability) : GameAction
{
    // TODO: Should this action have a target tile?
    // Or a target selector?
    public AbilityAttribute Ability { get; } = ability;
}
