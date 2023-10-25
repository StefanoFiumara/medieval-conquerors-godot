using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class PlayerSystem : GameComponent, IAwake, IDestroy
{
    private IEventAggregator _events;
    
    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        action.Target.MoveCards(action.CardsToDiscard, Zone.Discard);
    }
    
    public void Destroy()
    {
        _events.Unsubscribe(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
    }
}