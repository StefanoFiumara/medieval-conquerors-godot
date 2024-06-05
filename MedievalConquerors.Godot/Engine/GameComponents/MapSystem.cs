using Godot;
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
    private IGameMap _map;
    private Match _match;
    public static Vector2I InvalidTile => new Vector2I(int.MinValue, int.MinValue);

    public void Awake()
    {
        _events = Game.GetComponent<EventAggregator>();
        _map = Game.GetComponent<IGameMap>();
        _match = Game.GetComponent<Match>();
        
        _events.Subscribe<MoveUnitAction>(GameEvent.Perform<MoveUnitAction>(), OnPerformMoveUnit);
        _events.Subscribe<MoveUnitAction, ActionValidatorResult>(GameEvent.Validate<MoveUnitAction>(), OnValidateMoveUnit);
        
        // TODO: Unit tests for these actions/validations
        _events.Subscribe<PlayCardAction>(GameEvent.Perform<PlayCardAction>(), OnPerformPlayCard);
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayCard);
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayBuilding);
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayUnit);
        _events.Subscribe<PlayCardAction, ActionValidatorResult>(GameEvent.Validate<PlayCardAction>(), OnValidatePlayTechnology);
        
        _events.Subscribe<DiscardCardsAction>(GameEvent.Perform<DiscardCardsAction>(), OnPerformDiscardCards);
        _events.Subscribe<ChangeTurnAction>(GameEvent.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
    }

    private void OnPerformPlayCard(PlayCardAction action)
    {
        var tile = _map.GetTile(action.TargetTile);
        
        if (action.CardToPlay.CardData.CardType == CardType.Building)
            tile.Building = action.CardToPlay;


        else if (action.CardToPlay.CardData.CardType == CardType.Unit) 
            tile.Units.Add(action.CardToPlay);

        action.CardToPlay.MapPosition = action.TargetTile;
    }

    private void OnValidatePlayUnit(PlayCardAction action, ActionValidatorResult validator)
    {
        if (action.CardToPlay.CardData.CardType != CardType.Unit)
            return;
        
        var tile = _map.GetTile(action.TargetTile);
        
        // Units cannot be placed onto tiles that are not walkable
        if (!tile.IsWalkable) 
            validator.Invalidate("This tile is not walkable.");


        if (tile.Building == null) return;
        
        // Validate unit can be placed inside the tile's building
        // TODO: Generalize this, maybe with an attribute on the building?
        //       This currently only check economic and military, but other tags should be checked
        //       e.g. Mounted units can only be placed on or around stables
            
        var tileHasEconomicBuilding = tile.Building.CardData.Tags.HasFlag(Tags.Economic);
        var cardIsEconomicUnit =action.CardToPlay.CardData.Tags.HasFlag(Tags.Economic);

        if (tileHasEconomicBuilding && !cardIsEconomicUnit)
            validator.Invalidate("Only economic units can be placed inside economic buildings.");
            
        var tileHasMilitaryBuilding = tile.Building.CardData.Tags.HasFlag(Tags.Military);
        var cardIsMilitaryUnit =action.CardToPlay.CardData.Tags.HasFlag(Tags.Military);

        if (tileHasMilitaryBuilding && !cardIsMilitaryUnit)
            validator.Invalidate("Only military units can be placed inside military buildings.");
    }

    private void OnValidatePlayBuilding(PlayCardAction action, ActionValidatorResult validator)
    {
        if (action.CardToPlay.CardData.CardType != CardType.Building)
            return;
        
        var tile = _map.GetTile(action.TargetTile);
        if (tile.Building != null)
            validator.Invalidate("This tile already contains a building.");
    }

    private void OnValidatePlayTechnology(PlayCardAction action, ActionValidatorResult validator)
    {
        if (action.CardToPlay.CardData.CardType != CardType.Technology)
            return;
        
        var tile = _map.GetTile(action.TargetTile);
        if (tile.Building == null)
            validator.Invalidate("Technology card must be played on a building.");
        
        // TODO: Validate that this technology is valid for this building
        //       Probably do this with an attribute on the building?
    }

    private void OnValidatePlayCard(PlayCardAction action, ActionValidatorResult validator)
    {
        var tile = _map.GetTile(action.TargetTile);
        // TODO: Validate card can only be played within specific range from player's town center
    }

    private void OnPerformDiscardCards(DiscardCardsAction action)
    {
        foreach (var card in action.CardsToDiscard) 
        {
            if (card.MapPosition != InvalidTile)
            {
                var tile = _map.GetTile(card.MapPosition);
                
                if (card == tile.Building)
                    tile.Building = null;
                else
                    tile.Units.Remove(card);

                card.MapPosition = InvalidTile;
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
        var oldTile = _map.GetTile(action.CardToMove.MapPosition);
        var newTile = _map.GetTile(action.TargetTile);

        oldTile.Units.Remove(action.CardToMove);
        newTile.Units.Add(action.CardToMove);
        
        var distanceTraveled = _map.Distance(action.CardToMove.MapPosition, action.TargetTile);
        
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
                attr.Reset();
            }
        }
    }
}