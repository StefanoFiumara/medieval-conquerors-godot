namespace MedievalConquerors.Engine.Actions;

public class ShuffleDeckAction(int targetPlayerId) : GameAction
{
    public int TargetPlayerId { get; } = targetPlayerId;

    public override string ToString()
    {
        return $"ShuffleDeckAction:\tPlayer {TargetPlayerId} Shuffles their deck";
    }
}
