using Godot;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class AbilityAction(Card card, AbilityAttribute ability, Vector2I targetTile) : GameAction
{
    public Card Card { get; } = card;
    // TODO: Target tile can be removed when we implement target-less cards
    public Vector2I TargetTile { get; } = targetTile;
    public AbilityAttribute Ability { get; } = ability;
}
