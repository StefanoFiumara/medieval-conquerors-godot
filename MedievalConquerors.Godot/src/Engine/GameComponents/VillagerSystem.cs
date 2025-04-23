using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class VillagerSystem : GameComponent, IAwake {

    private EventAggregator _events;
    private Match _match;
    private CardLibrary _cardDb;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();
        _cardDb = Game.GetComponent<CardLibrary>();

        _events.Subscribe<BeginTurnAction>(GameEvent.Perform<BeginTurnAction>(), OnPerformBeginTurn);
    }

    private void OnPerformBeginTurn(BeginTurnAction action)
    {
        var player = _match.Players[action.PlayerId];

        var card = _cardDb.LoadCard(CardLibrary.VillagerId, player);
        player.Deck.Add(card);
    }
}
