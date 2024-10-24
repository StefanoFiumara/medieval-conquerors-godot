using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class AISystem : GameComponent, IAwake
{
    private Match _match;
    private ILogger _logger;

    private CardSystem _cardSystem;
    private TargetSystem _targetSystem;
    
    public void Awake()
    {
        _cardSystem = Game.GetComponent<CardSystem>();
        _targetSystem = Game.GetComponent<TargetSystem>();
        _match = Game.GetComponent<Match>();
        _logger = Game.GetComponent<ILogger>();
    }

    public void UseAction()
    {
        var nextAction = DecideAction();
        if (nextAction == null)
        {
            _logger.Info("*** AI ENDS TURN ***");
            Game.Perform(new ChangeTurnAction(1 - _match.CurrentPlayerId));
        }
        else
        {
            _logger.Info("*** AI ACTION ***");
            Game.Perform(nextAction);
        }
    }

    private GameAction DecideAction()
    {
        var playable = _match.CurrentPlayer.Hand
            .OrderByDescending(c => c.CardData.CardType == CardType.Building)
            .ThenByDescending(c => c.CardData.CardType == CardType.Unit && c.CardData.Tags.HasFlag(Tags.Economic))
            .ThenByDescending(c => c.CardData.CardType == CardType.Unit && c.CardData.Tags.HasFlag(Tags.Military));
        
        foreach (var card in playable)
        {
            if (_cardSystem.IsPlayable(card))
            {
                // TODO: Smarter tile selection
                var targetTile = _targetSystem.GetTargetCandidates(card).First();
                return new PlayCardAction(card, targetTile);
            }
        }
        
        return null;
    }
}