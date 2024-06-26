using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class GarrisonAction : GameAction
{
    public Card Unit { get; }
    public Card Building { get; }

    public GarrisonAction(Card unit, Card building)
    {
        SourcePlayer = unit.Owner;
        Unit = unit;
        Building = building;
    }
}