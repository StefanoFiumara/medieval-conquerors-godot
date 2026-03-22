namespace MedievalConquerors.Engine.Actions;

public class ResetSpentVillagersAction(int playerId) : GameAction
{
    public int PlayerId { get; private set; } = playerId;
}