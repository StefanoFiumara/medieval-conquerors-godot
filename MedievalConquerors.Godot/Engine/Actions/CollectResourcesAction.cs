namespace MedievalConquerors.Engine.Actions;

public class CollectResourcesAction : GameAction
{
    public int TargetPlayerId { get; }

    public CollectResourcesAction(int targetPlayerId)
    {
        TargetPlayerId = targetPlayerId;
    }
}