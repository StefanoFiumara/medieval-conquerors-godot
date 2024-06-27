using System;
using Godot;
using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Engine.Data;

public class ResourceBank
{
    public int Food { get; private set; }
    public int Wood { get; private set; }
    public int Gold { get; private set; }
    public int Stone { get; private set; }

    public int StorageLimit { get; private set; }

    private int TotalResources => Food + Wood + Gold + Stone;

    public ResourceBank(int storageLimit)
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

    public void IncreaseLimit(int amount)
    {
        StorageLimit += amount;
    }

    public void DecreaseLimit(int amount)
    {
        if (TotalResources != 0)
        {
            // TODO: double check this logic when testing storage limits
            int resourcesToRemove = TotalResources - StorageLimit + amount;
            float ratio = resourcesToRemove / (float)TotalResources;
        
            // TODO: should this floor instead of ceil? 
            Food -= Mathf.CeilToInt(Food * ratio);
            Wood -= Mathf.CeilToInt(Wood * ratio);
            Gold -= Mathf.CeilToInt(Gold * ratio);
            Stone -= Mathf.CeilToInt(Stone * ratio);
        }
        
        StorageLimit -= amount;
    }

    public int this[ResourceType resourceType]
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
            int availableSpace = StorageLimit - (TotalResources - this[resourceType]);
            int newAmount = (value > availableSpace) ? availableSpace : value;

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