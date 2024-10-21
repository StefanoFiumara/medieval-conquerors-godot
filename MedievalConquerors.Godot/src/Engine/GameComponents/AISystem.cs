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
    
    public void Awake()
    {
        _cardSystem = Game.GetComponent<CardSystem>();
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
            var action = DecideAction();
            Game.Perform(action);
        }
    }

    private GameAction DecideAction()
    {
        // TODO: return a GameAction for the AI to perform based on the current game state
        return null;
    }
}