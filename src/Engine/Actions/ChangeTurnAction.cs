namespace MedievalConquerors.Engine.Actions;

public class ChangeTurnAction(int nextPlayerId) : GameAction
{
    public int NextPlayerId { get; } = nextPlayerId;
}
