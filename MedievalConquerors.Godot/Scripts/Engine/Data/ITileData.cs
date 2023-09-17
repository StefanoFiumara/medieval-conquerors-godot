using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Engine.Data;

public interface ITileData
{
    List<IGameObject> Objects { get; }
    Vector2I Position { get; }
    TileTerrain Terrain { get; }
}

public class TileData : ITileData
{
    public List<IGameObject> Objects { get; } = new();
    public Vector2I Position { get; }
    public TileTerrain Terrain { get; }
	
    public TileData(Vector2I position, TileTerrain terrain)
    {
        Position = position;
        Terrain = terrain;
    }
}