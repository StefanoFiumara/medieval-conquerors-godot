using System;
using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Engine.Data;

public class ResourceBank
{
    public float Food { get; private set; }
    public float Wood { get; private set; }
    public float Gold { get; private set; }
    public float Stone { get; private set; }

    public float StorageLimit { get; private set; }

    private float TotalResources => Food + Wood + Gold + Stone;

    public ResourceBank(float storageLimit)
    {
        StorageLimit = storageLimit;
    }
    
    public bool CanAfford(ResourceCostAttribute resourceCost)
    {
        return    Food  >= resourceCost.Food
               && Wood  >= resourceCost.Wood
               && Gold  >= resourceCost.Gold
               && Stone >= resourceCost.Stone;
    }

    public void Subtract(ResourceCostAttribute resourceCost)
    {
        if (!CanAfford(resourceCost))
            throw new ArgumentException("Cannot afford resource cost");
        
        Food  -= resourceCost.Food;
        Wood  -= resourceCost.Wood;
        Gold  -= resourceCost.Gold;
        Stone -= resourceCost.Stone;
    }

    public void IncreaseLimit(float amount)
    {
        StorageLimit += amount;
    }

    public void DecreaseLimit(float amount)
    {
        if (TotalResources != 0)
        {
            float resourcesToRemove = TotalResources - StorageLimit + amount;
            float ratio = resourcesToRemove / TotalResources;
        
            Food -= Food * ratio;
            Wood -= Wood * ratio;
            Gold -= Gold * ratio;
            Stone -= Stone * ratio;
        }
        
        StorageLimit -= amount;
    }

    public float this[ResourceType resourceType]
    {
        get
        {
            return resourceType switch
            {
                ResourceType.Food => Food,
                ResourceType.Wood => Wood,
                ResourceType.Gold => Gold,
                ResourceType.Stone => Stone,
                _ => 0
            };
        }
        set
        {
            float availableSpace = StorageLimit - (TotalResources - this[resourceType]);
            float newAmount = (value > availableSpace) ? availableSpace : value;

            switch (resourceType)
            {
                case ResourceType.Food:
                    Food = newAmount;
                    break;
                case ResourceType.Wood:
                    Wood = newAmount;
                    break;
                case ResourceType.Gold:
                    Gold = newAmount;
                    break;
                case ResourceType.Stone:
                    Stone = newAmount;
                    break;
                default:
                    throw new ArgumentException("Invalid resource type");
            }
        }
    }
}