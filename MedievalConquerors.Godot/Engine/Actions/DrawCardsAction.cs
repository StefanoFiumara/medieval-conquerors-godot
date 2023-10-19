using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DrawCardsAction : GameAction
{
    public int Amount { get; }
    public List<Card> DrawnCards { get; set; }

    public DrawCardsAction(int amount)
    {
        Amount = amount;
        DrawnCards = new List<Card>();
    }
}