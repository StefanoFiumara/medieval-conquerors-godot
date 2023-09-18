using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MedievalConquerors.Engine.Data;

public interface ITileData
{
    List<IGameObject> Objects { get; }
    Vector2I Position { get; }
    TileTerrain Terrain { get; }
    ResourceType ResourceType { get; }
    int ResourceYield { get; }
    
    bool IsWalkable { get; } //  TODO: Better name?
}

public class TileData : ITileData
{
    public List<IGameObject> Objects { get; } = new();
    public Vector2I Position { get; }
    public TileTerrain Terrain { get; }
    public ResourceType ResourceType { get; }
    public int ResourceYield { get; }

    public bool IsWalkable => Terrain == TileTerrain.Grass && Objects.Count == 0;
	
    public TileData(Vector2I position, TileTerrain terrain, ResourceType resourceType, int resourceYield)
    {
        Position = position;
        Terrain = terrain;
        ResourceType = resourceType;
        ResourceYield = resourceYield;
    }
}