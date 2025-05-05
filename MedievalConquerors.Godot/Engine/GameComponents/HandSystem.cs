using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class HandSystem : GameComponent, IAwake
{
	private EventAggregator _events;
	private Match _match;

	public void Awake()
    {
	    _events = Game.GetComponent<EventAggregator>();
	    _match = Game.GetComponent<Match>();

	    _events.Subscribe<DrawCardsAction>(GameEvent.Perform<DrawCardsAction>(), OnPerformDrawCards);
    }

	private void OnPerformDrawCards(DrawCardsAction action)
	{
		var player = _match.Players[action.TargetPlayerId];
		var drawnCards = player.Deck.Draw(action.Amount);

		foreach (var card in drawnCards)
		{
			player.MoveCard(card, Zone.Hand);
			action.DrawnCards.Add(card);
		}
	}
}
