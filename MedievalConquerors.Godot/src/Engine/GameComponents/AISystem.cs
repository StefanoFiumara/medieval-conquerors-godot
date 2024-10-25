using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

public class AISystem : GameComponent, IAwake
{
    private Match _match;
    private ILogger _logger;

    private CardSystem _cardSystem;
    private TargetSystem _targetSystem;

    private HexMap _map;
    
    public void Awake()
    {
        _cardSystem = Game.GetComponent<CardSystem>();
        _targetSystem = Game.GetComponent<TargetSystem>();
        _map = Game.GetComponent<HexMap>();
        _match = Game.GetComponent<Match>();
        _logger = Game.GetComponent<ILogger>();
    }

    public void UseAction()
    {
        var nextAction = DecideAction();
        if (nextAction == null)
        {
            _logger.Info("*** AI ENDS TURN ***");
            Game.Perform(new ChangeTurnAction(1 - _match.CurrentPlayerId));
        }
        else
        {
            _logger.Info("*** AI ACTION ***");
            Game.Perform(nextAction);
        }
    }

    private GameAction DecideAction()
    {
        var playableCard = _match.CurrentPlayer.Hand
            .Where(c => _cardSystem.IsPlayable(c))
            .OrderByDescending(c => c.CardData.CardType == CardType.Building)
            .ThenByDescending(c => c.CardData.CardType == CardType.Unit && c.CardData.Tags.HasFlag(Tags.Economic))
            .ThenByDescending(c => c.CardData.CardType == CardType.Unit && c.CardData.Tags.HasFlag(Tags.Military))
            .FirstOrDefault();
        
        if (playableCard == null) return null;
        
        var targetTile = _targetSystem.GetTargetCandidates(playableCard).MaxBy(t => CalculateTileValue(playableCard, t));
        return new PlayCardAction(playableCard, targetTile);
        
        // TODO: Implement move and combat actions
    }

    private int CalculateTileValue(Card card, Vector2I tilePos)
    {
        int value = 0;
        
        if (card.CardData.CardType == CardType.Building && card.CardData.Tags.HasFlag(Tags.Economic))
            value += CalculateEconomicBuildingScore(card, tilePos);
        
        else if (card.CardData.CardType == CardType.Unit && card.CardData.Tags.HasFlag(Tags.Economic))
            value += CalculateEconomicUnitScore(card, tilePos);
        
        else if (card.CardData.CardType == CardType.Unit && card.CardData.Tags.HasFlag(Tags.Military))
            value += CalculateMilitaryUnitScore(card, tilePos);
        return value;
    }

    private int CalculateEconomicBuildingScore(Card card, Vector2I tilePos)
    {
        // the value of placing an economic building on this tile depends on whether the tile is adjacent to the resource it collects. 
        var resource = card.GetAttribute<ResourceCollectorAttribute>()?.Resource;
        if (resource == null) return 0;
        
        // score of the tile depends on how many resources of the type the card collects are adjacent to this tile. 
        var adjacentResourceCount = _map.GetNeighbors(tilePos).Where(t => t.ResourceType != ResourceType.None).Count(t => resource.Value.HasFlag(t.ResourceType));
        return adjacentResourceCount;
    }

    private int CalculateEconomicUnitScore(Card card, Vector2I tilePos)
    {
        // the value of placing an economic unit in this tile depends on whether the building in this tile has enough adjacent resource tiles to gather further resources.
        // TODO: See if building's gather rate can be used to further determine best placement
        // TODO: Determine if some resources are higher priority than others (e.g. we may want the AI to prioritize food over wood, or vice versa, this can also change depending on early/mid/late game. 
        var building = _map.GetTile(tilePos).Building;

        var resourceCollected = building?.GetAttribute<ResourceCollectorAttribute>()?.Resource;
        if (resourceCollected == null) return 0;

        var unitsGarrisoned = building.GetAttribute<GarrisonCapacityAttribute>()?.Units.Count ?? 0;

        // TODO: Test this logic with mining resources
        var adjacentResourceCount = _map.GetNeighbors(tilePos).Count(t => t.ResourceType.HasFlag(resourceCollected.Value));
        if (adjacentResourceCount == 0) return -10;

        return adjacentResourceCount - unitsGarrisoned;
    }
    
    private int CalculateMilitaryUnitScore(Card card, Vector2I tilePos)
    {
        // TODO: Come up with criteria for where to best place military units
        return 0;
    }
}