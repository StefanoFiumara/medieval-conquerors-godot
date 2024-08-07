using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.Data;

public class Player
{
    public int Id { get; }
    
    public TileData TownCenter { get; set; }
    public int InfluenceRange { get; set; }
    
    public ResourceBank Resources { get; }
    
    public List<Card> Deck    { get; } = new();
    public List<Card> Hand    { get; } = new();
    public List<Card> Discard { get; } = new();
    public List<Card> Map     { get; } = new();

    private readonly Dictionary<Zone, List<Card>> _zoneMap;

    public Player(int id)
    {
        Id = id;
        _zoneMap = new Dictionary<Zone, List<Card>>
        {
            { Zone.Deck, Deck },
            { Zone.Hand, Hand },
            { Zone.Discard, Discard },
            { Zone.Map, Map },
        };
        
        InfluenceRange = 3;

        Resources = new ResourceBank(storageLimit: 40 * 4)
        {
            // TODO: Parameterize Starting Resources
            [ResourceType.Food]  = 10,
            [ResourceType.Wood]  = 10,
            [ResourceType.Gold]  = 10,
            [ResourceType.Stone] = 10
        };
        // TODO: parameterize starting storage limit

        // TEMP: Add some temporary cards
        Deck.AddRange(Enumerable.Range(0, 4)
            .Select(i => CardBuilder.Build(this)
                .WithTitle("Villager")
                .WithDescription("Collects resources when assigned to gathering posts")
                .WithImagePath("res://Assets/CardImages/villager.png")
                .WithCardType(CardType.Unit)
                .WithTags(Tags.Economic)
                .WithResourceCost(food: 2)
                .WithSpawnPoint(Tags.Economic)
                .Create()));
        //
        // CardBuilder.Build(this)
        //         .WithTitle($"Knight {i}")
        //         .WithDescription($"Mighty Mounted Royal Warrior {i}")
        //         .WithImagePath("res://Assets/CardImages/knight.png")
        //         .WithCardType(CardType.Unit)
        //         .WithTags(Tags.Military | Tags.Mounted | Tags.Melee)
        //         .WithResourceCost(food: 4, gold: 2)
        //         .WithMovement(distance: 2)
        //         // TODO: Test with other buildings once available
        //         .WithSpawnPoint(Tags.TownCenter)
        //         .Create();
        
        Deck.Add(CardBuilder.Build(this)
            .WithTitle("Mining Camp")
            .WithDescription("Assigned villagers collect adjacent resources")
            .WithImagePath("res://Assets/CardImages/mining_camp.png")
            .WithCardType(CardType.Building)
            .WithTags(Tags.Economic)
            .WithResourceCost(wood: 2)
            .WithResourceCollector(ResourceType.Mining, gatherRate: 1f, storageLimitIncrease: 5)
            .WithGarrisonCapacity(capacity: 3)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());
        
        Deck.Add(CardBuilder.Build(this)
            .WithTitle("Mill")
            .WithDescription("Assigned villagers collect food from adjacent berries or farms.")
            .WithImagePath("res://Assets/CardImages/mill.png")
            .WithCardType(CardType.Building)
            .WithTags(Tags.Economic)
            .WithResourceCost(wood: 2)
            .WithResourceCollector(ResourceType.Food, gatherRate: 1f, storageLimitIncrease: 5)
            .WithGarrisonCapacity(capacity: 3)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());
    }
    
    public List<Card> this[Zone z] => _zoneMap.ContainsKey(z) ? _zoneMap[z] : null;

    public void MoveCard(Card target, Zone toZone)
    {
        var fromZone = this[target.Zone];
        var targetZone = this[toZone];
        
        fromZone?.Remove(target);
        targetZone?.Add(target);

        target.Zone = toZone;
    }

    public void MoveCards(List<Card> targets, Zone toZone)
    {
        foreach (var card in targets)
        {
            MoveCard(card, toZone);
        }
    }
}

