using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Views.Entities;
using MedievalConquerors.Views.Maps;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine.Input.InputStates;

public class PlayCardState : BaseInputState
{
    private List<Vector2I> _validTiles;
    private CardView _selectedCard;
    
    private readonly MapView _mapView;
    private readonly HandView _handView;

    public PlayCardState(IGame game, CardView selectedCard) : base(game)
    {
        _selectedCard = selectedCard;
        
        _mapView = game.GetComponent<MapView>();
        _handView = game.GetComponent<HandView>();
    }
    public override void Enter()
    {
        const float previewScale = 0.6f;
        const double tweenDuration = 0.3;
        // TODO: Formalize highlight colors in one file (Game settings?)
        _handView.SetSelected(_selectedCard);
        _selectedCard.Highlight(Colors.Cyan);
        
        
        var tween = _selectedCard.CreateTween().SetParallel().SetTrans(Tween.TransitionType.Sine);
        
        // TODO: Store the card's width/height as constants in the CardView class
        // TODO: Figure out if it's possible to derive these values from the scene itself
        tween.TweenProperty(_selectedCard, "global_position", Vector2.Zero + Vector2.Right * 135 * previewScale + Vector2.Down * 185 * previewScale, tweenDuration);
        tween.TweenProperty(_selectedCard, "scale", Vector2.One * previewScale, tweenDuration);
        
        // TODO: Use Target System to find valid tiles for placing the _selectedCard.
        //       Remove the dependency on IGameMap from this class.
        _validTiles = _mapView.GameMap.SearchTiles(t => t.IsWalkable).Select(t => t.Position).ToList();
        _mapView.HighlightTiles(_validTiles, HighlightLayer.TileSelectionHint);
    }

    public override void Exit()
    {
        _handView.ResetSelection();
        _selectedCard.RemoveHighlight();
        _mapView.RemoveHighlights(_validTiles, HighlightLayer.TileSelectionHint);
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
            return new WaitingForInputState(Game);
        
        if (IsPlayableCard(clickedObject) && clickedObject != _selectedCard)
            return Reselect((CardView)clickedObject);
   
        if (clickedObject is not TileData t)
            return this;
        
        if (!_validTiles.Contains(t.Position))
        {
            if (IsOwnedUnit(t))
                return new SelectUnitState(Game, t.Unit);
            // TODO: If invalid tile has a building, move to building selection (not implemented yet)
            
            return new WaitingForInputState(Game);
        }
        
        var action = new PlayCardAction(_selectedCard.Card, t.Position);
        Game.Perform(action);
        
        return new WaitingForInputState(Game);
    }
}