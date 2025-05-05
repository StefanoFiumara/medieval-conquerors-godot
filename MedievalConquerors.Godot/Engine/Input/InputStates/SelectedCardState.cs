using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Entities.Cards;
using MedievalConquerors.Entities.Hand;
using MedievalConquerors.Entities.Maps;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class SelectedCardState(IGame game, CardView selectedCard) : BaseInputState(game)
{
    private List<Vector2I> _validTiles;
    private CardView _selectedCard = selectedCard;

    private readonly MapView _mapView = game.GetComponent<MapView>();
    private readonly HandView _handView = game.GetComponent<HandView>();
    private readonly TargetSystem _targetSystem = game.GetComponent<TargetSystem>();

    public override void Enter()
    {
        _handView.SelectCardTween(_selectedCard);

        _validTiles = _targetSystem.GetTargetCandidates(_selectedCard.Card);
        _mapView.HighlightTiles(_validTiles, MapLayerType.SelectionHint);
    }

    public override void Exit()
    {
        _handView.ResetSelection();
        _selectedCard.RemoveHighlight();
        _mapView.RemoveHighlights(_validTiles, MapLayerType.SelectionHint);
    }

    private BaseInputState Reselect(CardView newCard)
    {
        Exit();
        _selectedCard = newCard;
        Enter();
        return this;
    }

    public override IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (mouseEvent.ButtonIndex == MouseButton.Right)
            return new IdleInputState(Game);

        if (IsPlayableCard(clickedObject) && clickedObject != _selectedCard)
            return Reselect((CardView)clickedObject);

        if (clickedObject is not TileData t)
            return this;

        if (!_validTiles.Contains(t.Position))
        {
            if (IsOwnedUnit(t))
                return new SelectedUnitState(Game, t.Unit);
            // TODO: If invalid tile has a building, move to building selection (not implemented yet)

            return new IdleInputState(Game);
        }

        var action = new PlayCardAction(_selectedCard.Card, t.Position);
        Game.Perform(action);

        return new IdleInputState(Game);
    }
}
