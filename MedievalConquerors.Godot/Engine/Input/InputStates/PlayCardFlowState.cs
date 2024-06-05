using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Views.Entities;
using MedievalConquerors.Views.Maps;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class PlayCardFlowState : IClickableState
{
    private readonly IGame _game;
    private readonly CardView _selectedCard;
    
    private readonly ILogger _logger;
    private readonly MapView _mapView;
    
    private List<Vector2I> _validTiles;

    public PlayCardFlowState(IGame game, CardView selectedCard)
    {
        _game = game;
        _selectedCard = selectedCard;
        
        _mapView = game.GetComponent<MapView>();
        _logger = game.GetComponent<ILogger>();
    }
    public void Enter()
    {
        // TODO: Use Target System to find valid tiles for placing the _selectedCard.
        //       Remove the dependency on IGameMap from this class.
        _validTiles = _mapView.GameMap.SearchTiles(t => t.IsWalkable).Select(t => t.Position).ToList();
        _mapView.HighlightTiles(_validTiles, HighlightLayer.TileSelectionHint);
    }

    public void Exit()
    {
        _mapView.RemoveHighlights(_validTiles, HighlightLayer.TileSelectionHint);
    }

    public IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (clickedObject is not TileData t)
            return this;

        _logger.Info($"Clicked on Tile {t.Position}");
        if (!_validTiles.Contains(t.Position))
        {
            _logger.Warn($"{t.Position} is not a valid tile for the selected card!");
            return this;
        }
        
        var action = new PlayCardAction(_selectedCard.Card, t.Position);
        _game.Perform(action);
        
        return new WaitingForInputState(_game);
    }
}