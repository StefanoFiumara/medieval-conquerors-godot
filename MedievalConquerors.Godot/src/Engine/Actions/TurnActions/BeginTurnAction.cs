namespace MedievalConquerors.Engine.Actions.TurnActions;

public class BeginTurnAction(int playerId) : GameAction
{
    public int PlayerId { get; private set; } = playerId;

    public override string ToString()
    {
        return $"BeginTurnAction:\tPlayer {PlayerId} Begins Their Turn";
    }
}
