using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class PlayCardAction(Card cardToPlay, Vector2I targetTile) : GameAction
{
    public Card CardToPlay { get; } = cardToPlay;
    public Vector2I TargetTile { get; } = targetTile;

    public override string ToString()
    {
        return $"PlayCardAction:\tPlayer {CardToPlay.Owner.Id} Plays {CardToPlay.Data.Title} at {TargetTile}";
    }
}
