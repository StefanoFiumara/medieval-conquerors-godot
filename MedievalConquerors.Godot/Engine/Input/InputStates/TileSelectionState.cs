using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Views.Entities;

namespace MedievalConquerors.Engine.Input.InputStates;

public class TileSelectionState : ITurnState
{
    private readonly CardView _selectedCard;
    private readonly IGame _game;
    private readonly ILogger _logger;

    public TileSelectionState(CardView selectedCard, IGame game)
    {
        _selectedCard = selectedCard;
        _game = game;
        _logger = game.GetComponent<ILogger>();
    }
    
    public void Enter() { }

    public void Exit() { }

    public ITurnState OnReceivedInput(IClickable clickedObject)
    {
        if (clickedObject is not TileData t)
            return this;

        _logger.Info($"Clicked on Tile {t.Position}");
        
        // TODO: Check if we clicked on a valid tile before executing action
        var action = new PlayCardAction(_selectedCard.Card, t.Position);
        _game.Perform(action);
        
        return new CardSelectionState(_game);
    }
}