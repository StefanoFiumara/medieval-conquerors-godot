using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this component
public class CardSystem : GameComponent, IAwake
{
    private readonly List<Card> _playable = new();

    private Match _match;
    private IGameMap _map;
    private ILogger _logger;

    public void Awake()
    {
        _logger = Game.GetComponent<ILogger>();
        _match = Game.GetComponent<Match>();
        _map = Game.GetComponent<IGameMap>();
    }

    public bool IsPlayable(Card card) => _playable.Contains(card);

    public void Refresh()
    {
        _playable.Clear();
        
        foreach (var card in _match.CurrentPlayer.Hand)
        {
            // TODO: more robust check for available tiles for this card, query attributes?
            var randomTargetTile = _map.SearchTiles(t => t.IsWalkable).GetRandom();
            var playAction = new PlayCardAction(card, randomTargetTile.Position);
            var validatorResult = playAction.Validate(Game);
            if (validatorResult.IsValid)
            {
                _playable.Add(card);
            }
            else
            {
                _logger.Warn($"Card in hand was invalidated, Reasons:\n{string.Join('\n', validatorResult.ValidationErrors)}");
            }
        }
    }
}