﻿using System;
using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Engine.Data;

public class ResourceBank
{
    public int Food { get; set; }
    public int Wood { get; set; }
    public int Gold { get; set; }
    public int Stone { get; set; }

    public bool CanAfford(ResourceCostAttribute resourceCost)
    {
        return Food >= resourceCost.Food
               && Wood >= resourceCost.Wood
               && Gold >= resourceCost.Gold
               && Stone >= resourceCost.Stone;
    }

    public void Subtract(ResourceCostAttribute resourceCost)
    {
        if (!CanAfford(resourceCost))
            throw new ArgumentException("Cannot afford resource cost");
        
        Food -= resourceCost.Food;
        Wood -= resourceCost.Wood;
        Gold -= resourceCost.Gold;
        Stone -= resourceCost.Stone;
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
            switch (resourceType)
            {
                case ResourceType.Food:
                    Food = value;
                    break;
                case ResourceType.Wood:
                    Wood = value;
                    break;
                case ResourceType.Gold:
                    Gold = value;
                    break;
                case ResourceType.Stone:
                    Stone = value;
                    break;
                default:
                    throw new ArgumentException("Invalid resource type");
            }
        }
    }
}