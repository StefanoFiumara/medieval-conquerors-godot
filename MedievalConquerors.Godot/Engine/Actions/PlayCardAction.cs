using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class PlayCardAction : GameAction
{
    public Card CardToPlay { get; }
    public Vector2I TargetTile { get; }

    public PlayCardAction(IPlayer sourcePlayer, Card cardToPlay, Vector2I targetTile)
    {
        SourcePlayer = sourcePlayer;
        CardToPlay = cardToPlay;
        TargetTile = targetTile;
    }
}