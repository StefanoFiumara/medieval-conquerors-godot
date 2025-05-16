using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Entities.Cards;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class IdleInputState(IGame game) : BaseInputState(game)
{
    public override void Enter() { }
    public override void Exit() { }

    public override IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (IsPlayableCard(clickedObject))
        {
            return new SelectedCardState(Game, (CardView)clickedObject);
        }

        if (IsOwnedUnit(clickedObject))
        {
            return new SelectedUnitState(Game, ((TileData) clickedObject).Unit);
        }

        return this;
    }
}
