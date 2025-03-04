using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DrawCardsAction(int targetPlayerId, int amount) : GameAction
{
    public int TargetPlayerId { get; } = targetPlayerId;
    public int Amount { get; } = amount;
    public List<Card> DrawnCards { get; set; } = new();

    public override string ToString()
    {
        return $"DrawCardsAction:\tPlayer {TargetPlayerId} Draws {Amount} card(s)";
    }
}
