using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class BanishCardsAction(List<Card> toBanish) : GameAction
{
    public List<Card> CardsToBanish { get; } = toBanish;
}