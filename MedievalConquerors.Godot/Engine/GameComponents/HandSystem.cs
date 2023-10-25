using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class HandSystem : GameComponent, IAwake, IDestroy
{
	private IEventAggregator _events;

	public void Awake()
    {
	    _events = Game.GetComponent<EventAggregator>();
	    _events.Subscribe<DrawCardsAction>(GameEvent.Perform<DrawCardsAction>(), OnPerformDrawCards);
	    
    }

	private void OnPerformDrawCards(DrawCardsAction action)
	{
		var drawnCards = action.Target.Deck.Draw(action.Amount);

		foreach (var card in drawnCards)
		{
			action.Target.MoveCard(card, Zone.Hand);
			action.DrawnCards.Add(card);
			card.Zone = Zone.Hand;
		}
	}

	public void Destroy()
    {
	    _events.Unsubscribe(GameEvent.Perform<DrawCardsAction>(), OnPerformDrawCards);
	    
    }
}