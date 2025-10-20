using System.Linq;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.GameComponents;

public class MovementSystem : GameComponent, IAwake
{
    private EventAggregator _events;
    private HexMap _map;
    private Match _match;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();
        _match = Game.GetComponent<Match>();

        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<MoveUnitAction>(GameEvent.Perform<MoveUnitAction>(), OnPerformMoveUnit);
        _events.Subscribe<MoveUnitAction, ActionValidatorResult>(GameEvent.Validate<MoveUnitAction>(), OnValidateMoveUnit);
        _events.Subscribe<BeginTurnAction>(GameEvent.Perform<BeginTurnAction>(), OnPerformBeginTurn);
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        if(action.CardToPlay.HasAttribute<MovementAttribute>())
            action.CardToPlay.ClearModifiers<MovementAttribute>();
    }

    private void OnValidateMoveUnit(MoveUnitAction action, ActionValidatorResult validator)
    {
        if (action.CardToMove.Zone != Zone.Map)
        {
            validator.Invalidate("Card is not on the map.");
            return;
        }

        if (action.CardToMove.HasAttribute<MovementAttribute>(out var moveAttr))
        {
            validator.Invalidate("Card does not have MoveAttribute.");
            return;
        }

        var distanceToTarget = _map.CalculatePath(action.CardToMove.MapPosition, action.TargetTile).Count;

        if (!moveAttr.CanMove(distanceToTarget))
            validator.Invalidate("Card's MoveAttribute does not have enough distance remaining.");
    }

    private void OnPerformMoveUnit(MoveUnitAction action)
    {
        var distanceTraveled = _map.CalculatePath(action.CardToMove.MapPosition, action.TargetTile).Count;

        var oldTile = _map.GetTile(action.CardToMove.MapPosition);
        var newTile = _map.GetTile(action.TargetTile);

        oldTile.Unit = null;
        newTile.Unit = action.CardToMove;

        action.CardToMove.MapPosition = action.TargetTile;
        var moveAttribute = action.CardToMove.GetAttribute<MovementAttribute>();

        action.CardToMove.AddModifier(new MovementModifier { RemainingDistance = moveAttribute.Distance - distanceTraveled });
    }

    private void OnPerformBeginTurn(BeginTurnAction action)
    {
        var player = _match.Players[action.PlayerId];

        foreach (var card in player.Map)
        {
            if (card.HasAttribute<MovementAttribute>())
                card.ClearModifiers<MovementAttribute>();
        }
    }
}
