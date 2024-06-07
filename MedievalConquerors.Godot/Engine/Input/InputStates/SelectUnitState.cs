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

public class SelectUnitState : BaseInputState
{
    private readonly MapView _mapView;
    
    private Card _selectedUnit;
    private List<Vector2I> _validTiles;

    public SelectUnitState(IGame game, Card selectedUnit) : base(game)
    {
        _selectedUnit = selectedUnit;

        _validTiles = new();
        
        _mapView = game.GetComponent<MapView>();
    }

    public override void Enter()
    {
        CalculateReachableTiles(_selectedUnit);
    }

    public override void Exit()
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
            _validTiles = _mapView.GameMap.GetReachable(_selectedUnit.MapPosition, movement.RemainingDistance).ToList();
        }
        
        // TODO: Should we have a separate layer/color for selected tiles vs Tile selection hints?
        _mapView.HighlightTile(_selectedUnit.MapPosition, HighlightLayer.TileSelectionHint);
        _mapView.HighlightTiles(_validTiles, HighlightLayer.TileSelectionHint);
    }

    public override IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (clickedObject is not TileData selectedTile) 
            return this;

        if (_validTiles.Contains(selectedTile.Position))
        {
            var action = new MoveUnitAction(_selectedUnit.Owner, _selectedUnit, selectedTile.Position);
            Game.Perform(action);

            return new WaitingForInputState(Game);
        }

        if (selectedTile.Position == _selectedUnit.MapPosition)
        {
            // Cancel selection
            return new WaitingForInputState(Game);
        }
        
        // TODO: Check this unit belongs to local player
        if (IsOwnedUnit(clickedObject))
        {
            CalculateReachableTiles(selectedTile.Unit);
            return this;
        }

        if (IsEnemyUnit(clickedObject))
        {
            // TODO: perform AttackUnitAction
        }
        
        //       Do we need a separate flow for that? If not, is MoveUnitFlowState the correct name for this state?
        return this;
    }
}