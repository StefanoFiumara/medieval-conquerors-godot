using System;

namespace MedievalConquerors.Engine.Data;

public enum TileTerrain
{
    None = 0,
    Water = 1,
    Grass = 2,
    Forest = 3,
    DeepWater = 4,
    TownCenter = 5, // NOTE: Only Used to dictate a player's starting TC
    ShoreFish = 6,
    Berries = 7,
    Deer = 8,
    Gold = 9,
    Stone = 10
}

public enum CardType
{
    None,
    Building,
    Unit,
    Technology
}

[Flags]
public enum Tags
{
    None = 0,
    Economic = 1 << 0,
    Military = 1 << 1,
    Infantry = 1 << 2,
    Melee = 1 << 3,
    Ranged = 1 << 4,
    Mounted = 1 << 5,
    TownCenter = 1 << 6 // NOTE: Special case for spawn point attributes
}

[Flags]
public enum ResourceType
{
    None = 0,
    Food = 1 << 0,  // 1
    Wood = 1 << 1,  // 2
    Gold = 1 << 2,  // 4
    Stone = 1 << 3, // 8
    Mining = Gold | Stone
}

public enum Zone
{
    None = 0,
    Deck = 1,
    Hand = 2,
    Map = 3,
    Discard = 4
}