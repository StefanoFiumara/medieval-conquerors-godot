using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Views.Entities;

namespace MedievalConquerors.Engine.Input.InputStates;

public class CardSelectionState : ITurnState
{
    private readonly IGame _game;
    private readonly ILogger _logger;
    private readonly CardSystem _cardSystem;

    public CardSelectionState(IGame game)
    {
        _game = game;
        _logger = game.GetComponent<ILogger>();
        _cardSystem = game.GetComponent<CardSystem>();
    }
    
    public void Enter() { }
    public void Exit() { }

    public ITurnState OnReceivedInput(IClickable clickedObject)
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