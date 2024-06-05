using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using MedievalConquerors.Views.Maps;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class MoveUnitFlowState : IClickableState
{
    private readonly IGame _game;
    
    private readonly MapView _mapView;
    private readonly IGameMap _map;
    
    private Card _selectedUnit;
    private List<Vector2I> _validTiles;

    public MoveUnitFlowState(IGame game, Card selectedUnit)
    {
        _game = game;
        _selectedUnit = selectedUnit;

        _validTiles = new();
        
        _mapView = game.GetComponent<MapView>();
        _map = game.GetComponent<IGameMap>();
    }

    public void Enter()
    {
        CalculateReachableTiles(_selectedUnit);
    }

    public void Exit()
    {
        _mapView.RemoveHighlight(_selectedUnit.MapPosition, HighlightLayer.TileSelectionHint);
        _mapView.RemoveHighlights(_validTiles, HighlightLayer.TileSelectionHint);
    }

    private void CalculateReachableTiles(Card unit)
    {
        _mapView.RemoveHighlight(_selectedUnit.MapPosition, HighlightLayer.TileSelectionHint);
        _mapView.RemoveHighlights(_validTiles, HighlightLayer.TileSelectionHint);

        _selectedUnit = unit;
        
        var movement = _selectedUnit.GetAttribute<MovementAttribute>();

        if (movement != null)
        {
            // TODO: Do we want to defer this logic to TargetSystem ? 
            _validTiles = _map.GetReachable(_selectedUnit.MapPosition, movement.RemainingDistance).ToList();
        }
        
        // TODO: Should we have a separate layer/color for selected tiles vs Tile selection hints?
        _mapView.HighlightTile(_selectedUnit.MapPosition, HighlightLayer.TileSelectionHint);
        _mapView.HighlightTiles(_validTiles, HighlightLayer.TileSelectionHint);
    }

    public IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (clickedObject is not TileData selectedTile) 
            return this;

        if (_validTiles.Contains(selectedTile.Position))
        {
            var action = new MoveUnitAction(_selectedUnit.Owner, _selectedUnit, selectedTile.Position);
            _game.Perform(action);

            return new WaitingForInputState(_game);
        }

        if (selectedTile.Position == _selectedUnit.MapPosition)
        {
            // Cancel selection
            return new WaitingForInputState(_game);
        }
        
        // TODO: Check this unit belongs to local player
        if (selectedTile.Unit != null)
        {
            CalculateReachableTiles(selectedTile.Unit);
            return this;
        }
        
        // TODO: If selected tile is enemy unit, perform AttackAction
        //       Do we need a separate flow for that? If not, is MoveUnitFlowState the correct name for this state?
        return this;
    }
}