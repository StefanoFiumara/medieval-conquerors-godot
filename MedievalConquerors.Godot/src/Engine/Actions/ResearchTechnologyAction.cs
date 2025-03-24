using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class ResearchTechnologyAction(Card card) : GameAction
{
    public Card Card { get; } = card;
}