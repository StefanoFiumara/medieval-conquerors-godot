using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class MapSystem : GameComponent, IAwake
{
    private IEventAggregator _events;
    private HexMap _map;
    private Match _match;

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<HexMap>();
        _match = Game.GetComponent<Match>();
        
        _events.Subscribe<MoveUnitAction>(GameEvent.Perform<MoveUnitAction>(), OnPerformMoveUnit);
        _events.Subscribe<MoveUnitAction, ActionValidatorResult>(GameEvent.Validate<MoveUnitAction>(), OnValidateMoveUnit);
        
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<ChangeTurnAction>(GameEvent.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        var tile = _map.GetTile(action.TargetTile);
        
        if (action.CardToPlay.CardData.CardType == CardType.Building)
            tile.Building = action.CardToPlay;
        
        else if (action.CardToPlay.CardData.CardType == CardType.Unit)
        {
            if (tile.Building != null)
                Game.AddReaction(new GarrisonAction(action.CardToPlay, tile.Building));
            else
                tile.Unit = action.CardToPlay;
        }

        action.CardToPlay.MapPosition = action.TargetTile;
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        foreach (var card in action.CardsToDiscard) 
        {
            if (card.MapPosition != HexMap.None)
            {
                var tile = _map.GetTile(card.MapPosition);
                
                if (card == tile.Building)
                    tile.Building = null;
                else
                    tile.Unit = null;

                card.MapPosition = HexMap.None;
            }
        }
    }
    
    private void OnValidateMoveUnit(MoveUnitAction action, ActionValidatorResult validator)
    {
        var moveAttr = action.CardToMove.GetAttribute<MovementAttribute>();
        
        if(action.CardToMove.Zone != Zone.Map)
            validator.Invalidate("Card is not on the map.");
        
        if(moveAttr == null)
            validator.Invalidate("Card does not have MoveAttribute.");

        else if (!moveAttr.CanMove(_map.Distance(action.CardToMove.MapPosition, action.TargetTile)))
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
        var moveAttr = action.CardToMove.GetAttribute<MovementAttribute>();
        moveAttr.Move(distanceTraveled);
    }
    
    private void OnPerformChangeTurn(ChangeTurnAction action)
    {
        var player = _match.Players[action.NextPlayerId];
        foreach (var card in player.Map)
        {
            foreach (var attr in card.Attributes)
            {
                attr.OnTurnStart();
            }
        }
    }
}