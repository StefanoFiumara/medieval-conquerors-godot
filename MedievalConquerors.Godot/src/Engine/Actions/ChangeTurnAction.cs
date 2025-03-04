namespace MedievalConquerors.Engine.Actions;

public class ChangeTurnAction(int nextPlayerId) : GameAction
{
    public int NextPlayerId { get; set; } = nextPlayerId;

    public override string ToString()
    {
        return $"ChangeTurnAction:\tPlayer {NextPlayerId}'s Turn";
    }
}
