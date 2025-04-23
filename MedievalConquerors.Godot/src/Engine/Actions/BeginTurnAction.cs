namespace MedievalConquerors.Engine.Actions;

public class BeginTurnAction(int playerId) : GameAction
{
    public int PlayerId { get; private set; } = playerId;

    public override string ToString()
    {
        return $"BeginTurnAction: Player {PlayerId} Begins Their Turn";
    }
}
