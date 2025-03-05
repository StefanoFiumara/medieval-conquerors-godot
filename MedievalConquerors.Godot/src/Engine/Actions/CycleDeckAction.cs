namespace MedievalConquerors.Engine.Actions;

public class CycleDeckAction(int targetPlayerId) : GameAction
{
    public int TargetPlayerId { get; } = targetPlayerId;
}
