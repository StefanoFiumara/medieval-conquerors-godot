using System.Linq;
using Godot;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class AISystem : GameComponent, IAwake
{
    private Match _match;
    private ILogger _logger;

    private CardSystem _cardSystem;
    private GarrisonSystem _garrisonSystem;
    private TargetSystem _targetSystem;

    private HexMap _map;

    public void Awake()
    {
        _cardSystem = Game.GetComponent<CardSystem>();
        _garrisonSystem = Game.GetComponent<GarrisonSystem>();
        _targetSystem = Game.GetComponent<TargetSystem>();
        _map = Game.GetComponent<HexMap>();
        _match = Game.GetComponent<Match>();
        _logger = Game.GetComponent<ILogger>();
    }

    public void UseAction()
    {
        _logger.Info("*** AI DECIDING ACTION ***");
        _logger.Debug($"AI's Hand: {string.Join(", ", _match.CurrentPlayer.Hand.Select(c => c.Data.Title))}");
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
        var playableCards = _match.CurrentPlayer.Hand
            .Where(c => _cardSystem.IsPlayable(c))
            .OrderByDescending(c => c.Data.CardType == CardType.Building)
            .ThenByDescending(c => c.Data.CardType == CardType.Unit && c.Data.Tags.HasFlag(Tags.Economic))
            .ThenByDescending(c => c.Data.CardType == CardType.Unit && c.Data.Tags.HasFlag(Tags.Military));

        foreach (var card in playableCards)
        {
            var target = _targetSystem.GetTargetCandidates(card)
                .Select(t => (Tile: t, Value: CalculateTileValue(card, t)))
                .MaxBy(t => t.Value);

            if (target.Value > 0)
                return new PlayCardAction(card, target.Tile);
        }

        // TODO: Implement move and combat actions
        return null;
    }

    private int CalculateTileValue(Card card, Vector2I tilePos)
    {
        int value = 0;

        if (card.Data.CardType == CardType.Building && card.Data.Tags.HasFlag(Tags.Economic))
            value += CalculateEconomicBuildingScore(card, tilePos);

        else if (card.Data.CardType == CardType.Unit && card.Data.Tags.HasFlag(Tags.Economic))
            value += CalculateEconomicUnitScore(card, tilePos);

        else if (card.Data.CardType == CardType.Unit && card.Data.Tags.HasFlag(Tags.Military))
            value += CalculateMilitaryUnitScore(card, tilePos);
        return value;
    }

    private int CalculateEconomicBuildingScore(Card card, Vector2I tilePos)
    {
        // TODO: Adjust scoring for other types of economic buildings, e.g. Farms do not have a resource collector attribute, so the AI never plays them.
        //       We need to adjust this logic to also account for any economic buildings with a resource provider attribute.

        // the value of placing an economic building on this tile depends on whether the tile is adjacent to the resource it collects.
        var resource = card.GetAttribute<ResourceProviderAttribute>()?.Resource;
        if (resource == null) return 0;

        return resource switch
        {
            ResourceType.Food => 5,
            ResourceType.Wood => 5,
            ResourceType.Gold => 4,
            // TODO: determine mining resource based on tilePos adjacent resources
            ResourceType.Mining => 3,
            ResourceType.Stone => 2,
            _ => 0
        };
    }

    private int CalculateEconomicUnitScore(Card card, Vector2I tilePos)
    {
        var building = _map.GetTile(tilePos).Building;

        var resource = building?.GetAttribute<ResourceProviderAttribute>()?.Resource;
        if (resource == null) return 0;

        var unitsGarrisoned = _garrisonSystem.GetGarrisonedUnits(building).Count;

        var resourceValue = resource switch
        {
            ResourceType.Food => 5,
            ResourceType.Wood => 5,
            ResourceType.Gold => 4,
            // TODO: determine mining resource based on tilePos adjacent resources
            ResourceType.Mining => 3,
            ResourceType.Stone => 2,
            _ => 0
        };

        // TODO: Test this logic with mining resources
        return resourceValue - unitsGarrisoned;
    }

    private int CalculateMilitaryUnitScore(Card card, Vector2I tilePos)
    {
        int value = 0;

        // TODO: How to rate smaller numbers higher while also deciding to NOT play a unit under some circumstances?
        var proximityToOwnBuildings = _map.GetNeighbors(tilePos)
            .Count(t => t.Building != null && t.Building.Owner == _match.CurrentPlayer);
        value += proximityToOwnBuildings;

        var proximityToEnemyUnits = _map.GetNeighbors(tilePos)
            .Count(t => t.Unit != null && t.Unit.Owner != _match.CurrentPlayer);
        value += proximityToEnemyUnits;

        var proximityToEnemyBuildings = _map.SearchTiles(t => t.Building != null && t.Building.Owner != _match.CurrentPlayer)
            .Select(t => _map.CalculatePath(tilePos, t.Position).Count)
            .Min();

        value += 10 - proximityToEnemyBuildings;
        return value;
    }
}
