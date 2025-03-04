using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DiscardCardsAction(List<Card> toDiscard, Player target) : GameAction
{
    public Player Target { get; } = target;
    public List<Card> CardsToDiscard { get; set; } = toDiscard;
}
