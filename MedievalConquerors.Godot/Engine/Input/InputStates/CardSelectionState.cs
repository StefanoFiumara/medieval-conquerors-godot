using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Views.Entities;

namespace MedievalConquerors.Engine.Input.InputStates;

public class CardSelectionState : IClickableState
{
    private readonly IGame _game;
    private readonly ILogger _logger;
    private readonly CardSystem _cardSystem;
    private readonly HandView _handView;

    public CardSelectionState(IGame game)
    {
        _game = game;
        _logger = game.GetComponent<ILogger>();
        _cardSystem = game.GetComponent<CardSystem>();
        _handView = game.GetComponent<HandView>();
    }

    public void Enter() { }
    public void Exit() { }

    public IClickableState OnReceivedInput(IClickable clickedObject, InputEventMouseButton mouseEvent)
    {
        if (clickedObject is not CardView view)
            return this;

        // TODO: Token selection for MoveAction?

        _logger.Info($"clicked on card: {view.Card.CardData.Title}");
        if (_cardSystem.IsPlayable(view.Card))
        {
            _logger.Info($"Card {view.Card.CardData.Title} is Playable!");
            return new TileSelectionState(view, _game);
        }
        
        return this;
    }
}