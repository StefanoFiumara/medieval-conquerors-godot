namespace MedievalConquerors.Engine.Data;

public enum TileTerrain
{
    Water = 0,
    Grass = 1,
    Forest = 2,
    DeepWater = 3,
    TownCenter = 4, // TEMP: Only Used to dictate a player's starting TC, possibly not needed
    ShoreFish = 5,
    Berries = 6,
    Deer = 7,
    Gold = 8,
    Stone = 9
}

public enum ResourceType
{
    None = -1,
    Food = 0,
    Wood = 1,
    Gold = 2,
    Stone = 3
}