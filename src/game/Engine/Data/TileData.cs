using Godot;
using MedievalConquerors.Engine.Input;

namespace MedievalConquerors.Engine.Data;

// TODO: Make immutable, track building/Unit somewhere else, perhaps just with their map position?
public class TileData(Vector2I position, TileTerrain terrain, ResourceType resourceType, int resourceYield) : IClickable
{
	public Card Building { get; set; }
	public Card Unit { get; set; }
	public Vector2I Position { get; } = position;
	public TileTerrain Terrain { get; } = terrain;
	public ResourceType ResourceType { get; } = resourceType;
	public int ResourceYield { get; } = resourceYield;

	public bool IsEmpty => Unit == null && Building == null;
	public bool IsWalkable => Terrain == TileTerrain.Grass && IsEmpty;
}
