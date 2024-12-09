using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.UI;

namespace MedievalConquerors.Engine.GameComponents;

public class TurnSystem : GameComponent, IAwake
{
    private Match _match;
    private IEventAggregator _events;
    private IGameSettings _settings;
    private CardRepository _db;

    public void Awake()
    {
        _match = Game.GetComponent<Match>();
        _events = Game.GetComponent<EventAggregator>();
        _settings = Game.GetComponent<IGameSettings>();
        _db = Game.GetComponent<CardRepository>();
            
        _events.Subscribe<ChangeTurnAction>(GameEvent.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        _events.Subscribe<BeginGameAction>(GameEvent.Perform<BeginGameAction>(), OnPerformBeginGame);
        _events.Subscribe(PlayerUiPanel.NextTurnClicked, OnClickNextTurn);
    }
    
    private void OnClickNextTurn()
    {
        if (!Game.IsIdle()) return;

        if (_match.CurrentPlayerId == _match.LocalPlayer.Id)
        {
            Game.Perform(new ChangeTurnAction(_match.EnemyPlayer.Id));
        }
    }
    
    private void OnPerformBeginGame(BeginGameAction action)
    {
        // Shuffle Decks
        Game.AddReaction(new ShuffleDeckAction(Match.LocalPlayerId));
        Game.AddReaction(new ShuffleDeckAction(Match.EnemyPlayerId));
        
        // Spawn Town Centers
        var townCenter1 = _db.LoadCard(CardRepository.TownCenterId, _match.LocalPlayer);
        var townCenter2 = _db.LoadCard(CardRepository.TownCenterId, _match.EnemyPlayer);
        Game.AddReaction(new PlayCardAction(townCenter1, _match.LocalPlayer.TownCenter.Position));
        Game.AddReaction(new PlayCardAction(townCenter2, _match.EnemyPlayer.TownCenter.Position));
        
        // Draw Starting Hand
        Game.AddReaction(new DrawCardsAction(Match.LocalPlayerId, _settings.StartingHandCount));
        Game.AddReaction(new DrawCardsAction(Match.EnemyPlayerId, _settings.StartingHandCount));
        
        // Change the turn to the starting player
        Game.AddReaction(new ChangeTurnAction(action.StartingPlayerId));
    }

    private void OnPerformChangeTurn(ChangeTurnAction action)
    {
        _match.CurrentPlayerId = action.NextPlayerId;
        Game.AddReaction(new DrawCardsAction(action.NextPlayerId, 1));
        Game.AddReaction(new CollectResourcesAction(action.NextPlayerId));
    }
}