using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Extensions;
using MedievalConquerors.Views;
using MapView = MedievalConquerors.Views.MapView;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class SelectUnitState(IGame game, Card selectedUnit) : BaseInputState(game)
{
    private readonly MapView _mapView = game.GetComponent<MapView>();
    private readonly HexMap _map = game.GetComponent<HexMap>();

    private Card _selectedUnit = selectedUnit;
    private List<Vector2I> _validTiles = new();

    public override void Enter()
    {
        var movement = _selectedUnit.GetAttribute<MovementAttribute>();
        if (movement != null)
            _validTiles = _map.GetReachable(_selectedUnit.MapPosition, movement.RemainingDistance).ToList();

        // TODO: Should we have a separate layer/color for selected tiles vs Tile selection hints?
        _mapView.HighlightTile(_selectedUnit.MapPosition, MapLayerType.SelectionHint);
        _mapView.HighlightTiles(_validTiles, MapLayerType.SelectionHint);
    }

    public override void Exit()
    {
        _mapView.RemoveHighlight(_selectedUnit.MapPosition, MapLayerType.SelectionHint);
        _mapView.RemoveHighlights(_validTiles, MapLayerType.SelectionHint);
    }

    private BaseInputState Reselect(Card unit)
    {
        Exit();
        _selectedUnit = unit;
        Enter();

        return this;
    }

    public override IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        // TODO: Switch to using right click for move/attack commands, left click for re-selection
        if (mouseEvent.ButtonIndex == MouseButton.Right)
            return new WaitingForInputState(Game);

        if (clickedObject is not TileData selectedTile)
            return this;

        if (selectedTile.Position == _selectedUnit.MapPosition)
            return new WaitingForInputState(Game);

        if (IsOwnedUnit(clickedObject))
            return Reselect(selectedTile.Unit);

        if (IsEnemyUnit(clickedObject))
        {
            // TODO: check enemy unit is in range for attack
            // TODO: perform AttackUnitAction
        }

        if (_validTiles.Contains(selectedTile.Position))
        {
            var action = new MoveUnitAction(_selectedUnit.Owner, _selectedUnit, selectedTile.Position);
            Game.Perform(action);

            return new WaitingForInputState(Game);
        }

        return this;
    }
}
