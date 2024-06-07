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

public class PlayCardState : BaseInputState
{
    private readonly ILogger _logger;
    
    private List<Vector2I> _validTiles;
    
    private readonly CardView _selectedCard;
    private readonly MapView _mapView;
    private readonly HandView _handView;

    public PlayCardState(IGame game, CardView selectedCard) : base(game)
    {
        _selectedCard = selectedCard;
        
        _mapView = game.GetComponent<MapView>();
        _handView = game.GetComponent<HandView>();
        _logger = game.GetComponent<ILogger>();
    }
    public override void Enter()
    {
        const float previewScale = 0.6f;
        const double tweenDuration = 0.3;
        // TODO: Formalize highlight colors in one file (Game settings?)
        _handView.SetSelected(_selectedCard);
        _selectedCard.Highlight(Colors.Cyan);
        
        var tween = _selectedCard.CreateTween().SetParallel().SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(_selectedCard, "global_position", Vector2.Zero + Vector2.Right * 135 * previewScale + Vector2.Down * 185 * previewScale, tweenDuration);
        tween.TweenProperty(_selectedCard, "scale", new Vector2(previewScale, previewScale), tweenDuration);
        
        // TODO: Use Target System to find valid tiles for placing the _selectedCard.
        //       Remove the dependency on IGameMap from this class.
        _validTiles = _mapView.GameMap.SearchTiles(t => t.IsWalkable).Select(t => t.Position).ToList();
        _mapView.HighlightTiles(_validTiles, HighlightLayer.TileSelectionHint);
    }

    public override void Exit()
    {
        // TODO: Test how this interacts when the action is canceled or another card is selected
        _handView.ResetSelection();
        _mapView.RemoveHighlights(_validTiles, HighlightLayer.TileSelectionHint);
    }

    public override IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        // TODO: If new card is selected, switch selected card and recalculate valid tiles
        // TODO: If selected card was clicked again, cancel selection
        if (clickedObject is not TileData t)
            return this;
        
        _logger.Info($"Clicked on Tile {t.Position}");
        if (!_validTiles.Contains(t.Position))
        {
            // TODO: If invalid tile has a unit, move to move unit flow state
            // TODO: If invalid tile has a building, move to building selection (not implemented yet)
            // TODO: Else, cancel selection
            _logger.Warn($"{t.Position} is not a valid tile for the selected card!");
            return this;
        }
        
        
        var action = new PlayCardAction(_selectedCard.Card, t.Position);
        Game.Perform(action);
        
        return new WaitingForInputState(Game);
    }
}