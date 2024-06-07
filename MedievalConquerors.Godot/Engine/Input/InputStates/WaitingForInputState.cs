using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Views.Entities;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class WaitingForInputState : BaseInputState
{
    

    public WaitingForInputState(IGame game) : base(game)
    {
        
    }
    
    public override void Enter() { }
    public override void Exit() { }

    public override IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (IsPlayableCard(clickedObject))
        {
            return new PlayCardState(Game, (CardView)clickedObject);
        }

        if (IsOwnedUnit(clickedObject))
        {
            // TODO: Validate that the selected unit belongs to the current player
            return new SelectUnitState(Game, ((TileData) clickedObject).Unit);
        }
        
        return this;
    }
}