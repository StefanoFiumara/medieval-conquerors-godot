using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class CardCreationSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private Match _match;
    private CardLibrary _cardDb;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _match = Game.GetComponent<Match>();
        _cardDb = Game.GetComponent<CardLibrary>();

        _events.Subscribe<CreateCardAction>(GameEvent.Perform<CreateCardAction>(), OnPerformCreateCard);
        _events.Subscribe<CreateCardAction, ActionValidatorResult>(GameEvent.Validate<CreateCardAction>(), OnValidateCreateCard);
    }

    private void OnValidateCreateCard(CreateCardAction action, ActionValidatorResult validator)
    {
        Zone[] allowedZones = [Zone.Deck, Zone.Hand, Zone.Discard];

        if(!_cardDb.IsValidCardId(action.CardId))
            validator.Invalidate($"Could not find card with ID {action.CardId} in library.");

        if (!allowedZones.Contains(action.TargetZone))
            validator.Invalidate("Cards can only be created in the Deck, Hand, or Discard zones.");
    }

    private void OnPerformCreateCard(CreateCardAction action)
    {
        var player = _match.Players[action.TargetPlayerId];
        for (int i = 0; i < action.Amount; i++)
        {
            var card = _cardDb.LoadCard(action.CardId, player);
            player[action.TargetZone].Add(card);
            action.CreatedCards.Add(card);
        }
    }
}
