using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class MoveUnitAction(Card cardToMove, Vector2I targetTile) : GameAction
{
    public Card CardToMove { get; } = cardToMove;
    public Vector2I TargetTile { get; } = targetTile;
}
