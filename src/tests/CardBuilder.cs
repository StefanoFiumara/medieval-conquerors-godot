using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Attributes;
using MedievalConquerors.Engine.Data;


namespace MedievalConquerors.Tests;

public class CardBuilder
{
    public static CardBuilder Build(Player owner)
    {
        return new CardBuilder(owner);
    }

    private readonly Player _owner;
    private int _id;
    private string? _title;
    private string? _description;
    private string? _imagePath;
    private string? _tokenImagePath;
    private CardType _cardType;
    private Tags _tags;
    private readonly List<ICardAttribute> _attributes = new();

    private CardBuilder(Player  owner)
    {
        _owner = owner;
    }

    public CardBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public CardBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CardBuilder WithImagePath(string imagePath)
    {
        _imagePath = imagePath;
        return this;
    }

    public CardBuilder WithTokenImagePath(string imagePath)
    {
        _tokenImagePath = imagePath;
        return this;
    }

    public CardBuilder WithCardType(CardType cardType)
    {
        _cardType = cardType;
        return this;
    }

    public CardBuilder WithTags(Tags tags)
    {
        _tags = tags;
        return this;
    }

    public CardBuilder WithResourceCollector(ResourceType resource)
    {
        _attributes.Add(new ResourceCollectorAttribute { Resource = resource });
        return this;
    }

    public CardBuilder WithResourceCost(int food = 0, int wood = 0, int gold = 0, int stone = 0)
    {
        _attributes.Add(new ResourceCostAttribute
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
        _attributes.Add(new MovementAttribute { Distance = distance });
        return this;
    }

    public CardBuilder WithSpawnPoint(Tags spawnTags, int spawnRange = 0)
    {
        _attributes.Add(new SpawnPointAttribute
        {
            SpawnRange = spawnRange,
            SpawnTags = spawnTags
        });

        return this;
    }

    public CardBuilder WithAbility<TAbility, TAction>(string data = "")
    where TAbility : AbilityAttribute, new()
    where TAction : GameAction, IAbilityLoader, new()
    {
        var ability = new TAbility
        {
            Actions = [new ActionDefinition { ActionType = typeof(TAction).FullName, Data = data }]
        };
        _attributes.Add(ability);

        return this;
    }

    public CardBuilder WithGarrisonCapacity(int capacity)
    {
        _attributes.Add(new GarrisonCapacityAttribute
        {
            Limit = capacity
        });

        return this;
    }

    public Card Create()
    {
        var data = new CardData
        {
            Id = _id,
            Title = _title,
            Description = _description,
            ImagePath = _imagePath,
            TokenImagePath = _tokenImagePath,
            CardType = _cardType,
            Tags = _tags,
            Attributes = _attributes
        };
        return new Card(data, _owner);
    }

    public List<Card> CreateMany(int count) => Enumerable.Range(0, count).Select(_ => Create()).ToList();
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
                .WithCardType(CardType.Unit)
                .WithTags(Tags.Economic)
                .WithResourceCost(food: 2)
                .WithSpawnPoint(Tags.Economic)
                .Create()));

        cards.Add(CardBuilder.Build(owner)
            .WithTitle($"Knight")
            .WithDescription($"Mighty Mounted Royal Warrior")
            .WithCardType(CardType.Unit)
            .WithTags(Tags.Military | Tags.Mounted | Tags.Melee)
            .WithResourceCost(food: 4, gold: 2)
            .WithMovement(distance: 2)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());

        cards.Add(CardBuilder.Build(owner)
            .WithTitle($"Swordsman")
            .WithDescription($"Standard foot soldier equipped with a sword")
            .WithCardType(CardType.Unit)
            .WithTags(Tags.Military | Tags.Infantry | Tags.Melee)
            .WithResourceCost(food: 4, gold: 2)
            .WithMovement(distance: 1)
            .WithSpawnPoint(Tags.TownCenter)
            .Create());

        cards.Add(CardBuilder.Build(owner)
            .WithTitle("Lumber Camp")
            .WithDescription("Assigned villagers collect adjacent resources")
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
