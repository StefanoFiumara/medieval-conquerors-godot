using System;

namespace MedievalConquerors.Engine.Data;

public enum TileTerrain
{
    None = 0,
    Water = 1,
    Grass = 2,
    Forest = 3,
    Berries = 4,
    Deer = 5,
    Gold = 6,
    Stone = 7,
    StartingTownCenterBlue = 8, // NOTE: Only Used to dictate a player's starting TC
    StartingTownCenterRed = 9, // NOTE: Only Used to dictate a player's starting TC
}

public enum CardType
{
    None = 0,
    Building = 1,
    Unit = 2,
    Technology = 3,
    Action = 4
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
    Food = 1 << 0,
    Wood = 1 << 1,
    Gold = 1 << 2,
    Stone = 1 << 3,
    Mining = Gold | Stone
}

public enum AgeType
{
    None = 0,
    DarkAge = 1,
    FeudalAge = 2,
    CastleAge = 3,
    ImperialAge = 4
}

public enum Zone
{
    None = 0,
    Deck = 1,
    Hand = 2,
    Map = 3,
    Discard = 4,
    Banished = 5
}

public enum MapLayerType
{
    Terrain = 0,
    MouseHover = 1,
    SelectionHint = 2
}

public enum PlayerTarget
{
    None,
    Owner,
    Enemy
}

public static class EnumExtensions
{
    public static string PrettyPrint(this AgeType age)
    {
        return age switch
        {
            AgeType.None => string.Empty,
            AgeType.DarkAge => "I - Dark Age",
            AgeType.FeudalAge => "II - Feudal Age",
            AgeType.CastleAge => "III - Castle Age",
            AgeType.ImperialAge => "IV - Imperial Age",
            _ => throw new ArgumentOutOfRangeException(nameof(age))
        };
    }
}
