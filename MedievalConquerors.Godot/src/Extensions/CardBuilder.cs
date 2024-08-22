using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Extensions;

// TODO: Move this to the test project when we're no longer using it in Player.cs 
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

    public CardBuilder WithResourceCollector(ResourceType resource, float gatherRate, int storageLimitIncrease)
    {
        _data.Attributes.Add(new ResourceCollectorAttribute
        {
            Resource = resource,
            GatherRate = gatherRate,
            StorageLimitIncrease = storageLimitIncrease
        });

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

    public Card Create() => new Card(_data, _owner);
}