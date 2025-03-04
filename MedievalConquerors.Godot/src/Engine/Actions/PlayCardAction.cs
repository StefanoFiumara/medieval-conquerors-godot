using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class PlayCardAction : GameAction
{
    public Card CardToPlay { get; }
    public Vector2I TargetTile { get; }

    public PlayCardAction(Card cardToPlay, Vector2I targetTile)
    {
        SourcePlayer = cardToPlay.Owner;
        CardToPlay = cardToPlay;
        TargetTile = targetTile;
    }

    public override string ToString()
    {
        return $"PlayCardAction: Player {SourcePlayer.Id} plays {CardToPlay.CardData.Title} at {TargetTile}";
    }
}
