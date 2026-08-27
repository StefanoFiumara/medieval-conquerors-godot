using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class PlayCardAction(Card cardToPlay, Vector2I targetTile) : GameAction
{
    public Card CardToPlay { get; } = cardToPlay;
    // TODO: Not all cards will use target tile
    //       Previously, the target selector attribute would store which tile was targeted
    //       But now attributes are no longer stateful, so where should we store that?
    //       We could just make this optional, but I wonder if there is a better approach.
    public Vector2I TargetTile { get; } = targetTile;

    public override string ToString()
    {
        return $"PlayCardAction:\tPlayer {CardToPlay.Owner.Id} Plays {CardToPlay.Data.Title} at {TargetTile}";
    }
}
