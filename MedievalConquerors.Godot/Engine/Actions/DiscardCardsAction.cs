using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DiscardCardsAction : GameAction
{
    public IPlayer Target { get; }
    public List<Card> CardsToDiscard { get; set; }

    public DiscardCardsAction(List<Card> toDiscard, IPlayer target)
    {
        CardsToDiscard = toDiscard;
        Target = target;
    }
}