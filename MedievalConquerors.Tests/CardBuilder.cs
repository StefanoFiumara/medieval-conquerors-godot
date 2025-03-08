using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Tests;

public class CardBuilder
{
    public static CardBuilder Build(Player owner)
    {
        return new CardBuilder(owner);
    }

    private readonly Player _owner;
    private readonly CardData _data;

    private CardBuilder(Player  owner)
    {
        _owner = owner;
        _data = new CardData();
    }

    public CardBuilder WithTitle(string title)
    {
        _data.Title = title;
        return this;
    }

    public CardBuilder WithDescription(string description)
    {
        _data.Description = description;
        return this;
    }

    public CardBuilder WithImagePath(string imagePath)
    {
        _data.ImagePath = imagePath;
        return this;
    }

    public CardBuilder WithTokenImagePath(string imagePath)
    {
        _data.TokenImagePath = imagePath;
        return this;
    }

    public CardBuilder WithCardType(CardType cardType)
    {
        _data.CardType = cardType;
        return this;
    }

    public CardBuilder WithTags(Tags tags)
    {
        _data.Tags = tags;
        return this;
    }

    public CardBuilder WithResourceCollector(ResourceType resource)
    {
        _data.Attributes.Add(new ResourceCollectorAttribute { Resource = resource });
        return this;
    }

    public CardBuilder WithResourceCost(int food = 0, int wood = 0, int gold = 0, int stone = 0)
    {
        _data.Attributes.Add(new ResourceCostAttribute
        {
            Food = food,
            Wood = wood,
            Gold = gold,
            Stone = stone
        });

        return this;
    }

    public CardBuilder WithMovement(int distance = 0)
    {
        _data.Attributes.Add(new MovementAttribute { Distance = distance });
        return this;
    }

    public CardBuilder WithHealthPoints(int health)
    {
        _data.Attributes.Add(new HitPointsAttribute { Health = health });
        return this;
    }

    public CardBuilder WithSpawnPoint(Tags spawnTags, int spawnRange = 0)
    {
        _data.Attributes.Add(new SpawnPointAttribute
        {
            SpawnRange = spawnRange,
            SpawnTags = spawnTags
        });

        return this;
    }

    public CardBuilder WithGarrisonCapacity(int capacity)
    {
        _data.Attributes.Add(new GarrisonCapacityAttribute
        {
            Limit = capacity
        });

        return this;
    }

    public Card Create() => new(_data, _owner);
    public List<Card> CreateMany(int count) => Enumerable.Range(0, count).Select(_ => new Card(_data, _owner)).ToList();
}

public static class DeckBuilder
{
    public static List<Card> CreateTestDeck(Player owner)
    {
        var cards = new List<Card>();
        cards.AddRange(Enumerable.Range(0, 15)
            .Select(_ => CardBuilder.Build(owner)
                .WithTitle("Villager")
                .WithDescription("Collects resources when assigned to gathering posts")
                .WithImagePath("res://assets/portraits/villager.png")
                .WithTokenImagePath("res://assets/tile_tokens/villager.png")
                .WithCardType(CardType.Unit)
                .WithTags(Tags.Economic)
                .WithResourceCost(food: 2)
                .WithSpawnPoint(Tags.Economic)
                .Create()));

        cards.Add(CardBuilder.Build(owner)
            .WithTitle($"Knight")
            .WithDescription($"Mighty Mounted Royal Warrior")
            .WithImagePath("res://assets/portraits/knight.png")
            .WithTokenImagePath("res://assets/tile_tokens/knight.png")
            .WithCardType(CardType.Unit)
            .WithTags(Tags.Military | Tags.Mounted | Tags.Melee)
            .WithResourceCost(food: 4, gold: 2)
            .WithMovement(distance: 2)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());

        cards.Add(CardBuilder.Build(owner)
            .WithTitle($"Swordsman")
            .WithDescription($"Standard foot soldier equipped with a sword")
            .WithImagePath("res://assets/portraits/missing_icon.png")
            .WithTokenImagePath("res://assets/tile_tokens/swordsman.png")
            .WithCardType(CardType.Unit)
            .WithTags(Tags.Military | Tags.Infantry | Tags.Melee)
            .WithResourceCost(food: 4, gold: 2)
            .WithMovement(distance: 1)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());

        cards.Add(CardBuilder.Build(owner)
            .WithTitle("Lumber Camp")
            .WithDescription("Assigned villagers collect adjacent resources")
            .WithImagePath("res://assets/portraits/lumber_camp.png")
            .WithTokenImagePath("res://assets/tile_tokens/lumber_camp.png")
            .WithCardType(CardType.Building)
            .WithTags(Tags.Economic)
            .WithResourceCost(wood: 2)
            .WithResourceCollector(ResourceType.Wood)
            .WithGarrisonCapacity(capacity: 3)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());

        cards.Add(CardBuilder.Build(owner)
            .WithTitle("Mining Camp")
            .WithDescription("Assigned villagers collect adjacent resources")
            .WithImagePath("res://assets/portraits/mining_camp.png")
            .WithTokenImagePath("res://assets/tile_tokens/mining_camp.png")
            .WithCardType(CardType.Building)
            .WithTags(Tags.Economic)
            .WithResourceCost(wood: 2)
            .WithResourceCollector(ResourceType.Mining)
            .WithGarrisonCapacity(capacity: 3)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());

        cards.Add(CardBuilder.Build(owner)
            .WithTitle("Mill")
            .WithDescription("Assigned villagers collect food from adjacent berries or farms.")
            .WithImagePath("res://assets/portraits/mill.png")
            .WithTokenImagePath("res://assets/tile_tokens/mill.png")
            .WithCardType(CardType.Building)
            .WithTags(Tags.Economic)
            .WithResourceCost(wood: 2)
            .WithResourceCollector(ResourceType.Food)
            .WithGarrisonCapacity(capacity: 3)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());

        return cards;
    }
}
