using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class GarrisonAction(Card unit, Card building) : GameAction
{
    public Card Unit { get; } = unit;
    public Card Building { get; } = building;
}
