namespace MedievalConquerors.Engine.Actions;

public class ShuffleDeckAction : GameAction
{
    public int TargetPlayerId { get; }

    public ShuffleDeckAction(int targetPlayerId)
    {
        TargetPlayerId = targetPlayerId;
    }
}