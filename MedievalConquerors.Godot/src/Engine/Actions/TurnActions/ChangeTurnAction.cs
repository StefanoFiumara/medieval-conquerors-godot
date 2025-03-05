namespace MedievalConquerors.Engine.Actions.TurnActions;

public class ChangeTurnAction(int nextPlayerId) : GameAction
{
    public int NextPlayerId { get; } = nextPlayerId;
}
