using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class AbilityAction(Card card, AbilityAttribute ability, Vector2I targetTile) : GameAction
{
    // TODO: Should this action have a target tile?
    // Or a target selector?
    public Card Card { get; } = card;
    public Vector2I TargetTile { get; } = targetTile;
    public AbilityAttribute Ability { get; } = ability;
}
