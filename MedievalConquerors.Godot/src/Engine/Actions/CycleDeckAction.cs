namespace MedievalConquerors.Engine.Actions;

public class CycleDeckAction(int targetPlayerId) : GameAction
{
    public int TargetPlayerId { get; } = targetPlayerId;

    public override string ToString()
    {
        return $"CycleDeckAction: Player {TargetPlayerId} shuffles their discard pile into their deck.";
    }
}
