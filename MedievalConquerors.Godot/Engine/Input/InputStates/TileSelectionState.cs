using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Views.Entities;

namespace MedievalConquerors.Engine.Input.InputStates;

public class TileSelectionState : ITurnState
{
    private readonly CardView _selectedCard;
    private readonly IGame _game;
    private readonly ILogger _logger;

    public TileSelectionState(CardView selectedCard, IGame game, ILogger logger)
    {
        _selectedCard = selectedCard;
        _game = game;
        _logger = logger;
    }
    
    public void Enter() { }

    public void Exit() { }

    public ITurnState OnReceivedInput(IClickable clickedObject)
    {
        if (clickedObject is not TileData t)
            return this;

        _logger.Info($"Clicked on Tile {t.Position}");
        return new CardSelectionState(_game, _logger);
    }
}