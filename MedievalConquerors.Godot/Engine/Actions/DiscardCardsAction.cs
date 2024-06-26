using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DiscardCardsAction : GameAction
{
    public Player Target { get; }
    public List<Card> CardsToDiscard { get; set; }

    public DiscardCardsAction(List<Card> toDiscard, Player target)
    {
        CardsToDiscard = toDiscard;
        Target = target;
    }
}