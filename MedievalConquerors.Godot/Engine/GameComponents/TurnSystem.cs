using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class TurnSystem : GameComponent, IAwake
{
    private Match _match;
    private IEventAggregator _events;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _events = Game.GetComponent<IEventAggregator>();
            
        _events.Subscribe<ChangeTurnAction>(GameEvent.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        _events.Subscribe<BeginGameAction>(GameEvent.Perform<BeginGameAction>(), OnPerformBeginGame);
    }

    private void OnPerformBeginGame(BeginGameAction action)
    {
        Game.AddReaction(new ShuffleDeckAction(Match.LocalPlayerId));
        Game.AddReaction(new ShuffleDeckAction(Match.EnemyPlayerId));
        
        Game.AddReaction(new DrawCardsAction(Match.LocalPlayerId, 5));
        Game.AddReaction(new DrawCardsAction(Match.EnemyPlayerId, 5));
        
        Game.AddReaction(new ChangeTurnAction(action.StartingPlayerId));
    }

    private void OnPerformChangeTurn(ChangeTurnAction action)
    {
        _match.CurrentPlayerId = action.NextPlayerId;
        Game.AddReaction(new DrawCardsAction(action.NextPlayerId, 1));
    }
}