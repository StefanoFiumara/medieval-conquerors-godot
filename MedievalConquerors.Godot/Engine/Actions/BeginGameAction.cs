namespace MedievalConquerors.Engine.Actions;

public class BeginGameAction : GameAction
{
    public int StartingPlayerId { get; set; }
    
    public BeginGameAction(int startingPlayerId)
    {
        StartingPlayerId = startingPlayerId;
    }
}