namespace MedievalConquerors.Engine.Actions;

public class ChangeTurnAction : GameAction
{
    public int NextPlayerId { get; set; }

    public ChangeTurnAction(int nextPlayerId)
    {
        NextPlayerId = nextPlayerId;
    }
}