using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class MoveUnitAction : GameAction
{
    public Card CardToMove { get; }
    public Vector2I TargetTile { get; }

    public MoveUnitAction(IPlayer sourcePlayer, Card cardToMove, Vector2I targetTile)
    {
        SourcePlayer = sourcePlayer;
        CardToMove = cardToMove;
        TargetTile = targetTile;
    }
}