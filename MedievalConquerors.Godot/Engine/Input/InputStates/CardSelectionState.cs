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

    public CardSelectionState(IGame game, ILogger logger)
    {
        _game = game;
        _logger = logger;
        _cardSystem = game.GetComponent<CardSystem>();
    }
    
    public void Enter() { }
    public void Exit() { }

    public ITurnState OnReceivedInput(IClickable clickedObject)
    {
        if (clickedObject is not CardView c)
            return this;

        // TODO: Token selection for MoveAction?

        _logger.Info($"clicked on card: {c.Card.CardData.Title}");
        if (_cardSystem.IsPlayable(c.Card))
        {
            _logger.Info($"Card {c.Card.CardData.Title} is Playable!");
            return new TileSelectionState(c, _game, _logger);
        }
        
        return this;
    }
}