using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DiscardCardsAction(List<Card> toDiscard) : GameAction
{
    public List<Card> CardsToDiscard { get; } = toDiscard;
}
