using Godot;
using MedievalConquerors.Engine.Attributes;

namespace MedievalConquerors.Engine.Actions;

public class AbilityAction(AbilityAttribute ability, Vector2I targetTile) : GameAction
{
    // TODO: Should this action have a target tile?
    // Or a target selector?
    public Vector2I TargetTile { get; } = targetTile;
    public AbilityAttribute Ability { get; } = ability;
}
