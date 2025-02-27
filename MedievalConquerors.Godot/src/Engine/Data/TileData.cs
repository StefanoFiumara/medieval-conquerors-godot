using Godot;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Engine.Data;

public class TileData : IClickable
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