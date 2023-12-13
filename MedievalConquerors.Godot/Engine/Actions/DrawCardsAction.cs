using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DrawCardsAction : GameAction
{
    public int TargetPlayerId { get; }
    public int Amount { get; }
    public List<Card> DrawnCards { get; set; }

    public DrawCardsAction(int targetPlayerId, int amount)
    {
        TargetPlayerId = targetPlayerId;
        Amount = amount;
        DrawnCards = new List<Card>();
    }
}