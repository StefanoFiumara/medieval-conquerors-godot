using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Engine.Data;

public interface ITileData
{
    Card Building { get; set; }
    Card Unit { get; set; }
    Vector2I Position { get; }
    TileTerrain Terrain { get; }
    ResourceType ResourceType { get; }
    int ResourceYield { get; }
    
    bool IsEmpty { get; }
    bool IsWalkable { get; } //  TODO: Better name?
}

public class TileData : ITileData, IClickable
{
    public Card Building { get; set; }
    public Card Unit { get; set; }
    public Vector2I Position { get; }
    public TileTerrain Terrain { get; }
    public ResourceType ResourceType { get; }
    public int ResourceYield { get; }

    public bool IsEmpty => Unit == null && Building == null;
    public bool IsWalkable => Terrain == TileTerrain.Grass && IsEmpty;
	
    public TileData(Vector2I position, TileTerrain terrain, ResourceType resourceType, int resourceYield)
    {
        Position = position;
        Terrain = terrain;
        ResourceType = resourceType;
        ResourceYield = resourceYield;
    }
}