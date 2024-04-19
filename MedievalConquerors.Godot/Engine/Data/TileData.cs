using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Engine.Data;

public interface ITileData
{
    Card Building { get; set; }
    List<Card> Units { get; }
    Vector2I Position { get; }
    TileTerrain Terrain { get; }
    ResourceType ResourceType { get; }
    int ResourceYield { get; }
    
    bool IsWalkable { get; } //  TODO: Better name?
}

public class TileData : ITileData, IClickable
{
    public Card Building { get; set; }
    public List<Card> Units { get; }
    public Vector2I Position { get; }
    public TileTerrain Terrain { get; }
    public ResourceType ResourceType { get; }
    public int ResourceYield { get; }

    public bool IsWalkable => Terrain == TileTerrain.Grass && Units.Count == 0;
	
    public TileData(Vector2I position, TileTerrain terrain, ResourceType resourceType, int resourceYield)
    {
        Position = position;
        Terrain = terrain;
        ResourceType = resourceType;
        ResourceYield = resourceYield;

        Units = new List<Card>();
    }
}