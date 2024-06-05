using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Views.Entities;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class WaitingForInputState : IClickableState
{
    private readonly IGame _game;
    private readonly CardSystem _cardSystem;

    public WaitingForInputState(IGame game)
    {
        _game = game;
        _cardSystem = game.GetComponent<CardSystem>();
    }
    
    public void Enter() { }
    public void Exit() { }

    public IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (clickedObject is CardView cardView && _cardSystem.IsPlayable(cardView.Card))
        {
            return new PlayCardFlowState(_game, cardView);
        }

        // TODO: Should we use CardSystem to keep track of which cards can be moved?
        //       Alternatively, we can introduce MovementSystem to handle it.
        if (clickedObject is TileData tile && tile.Unit != null)
        {
            // TODO: Validate that the selected unit belongs to the current player
            return new MoveUnitFlowState(_game, tile.Unit);
        }
        
        return this;
    }
}