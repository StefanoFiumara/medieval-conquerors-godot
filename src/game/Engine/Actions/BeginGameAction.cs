namespace MedievalConquerors.Engine.Actions;

public class BeginGameAction(int startingPlayerId) : GameAction
{
    public int StartingPlayerId { get; } = startingPlayerId;
}
