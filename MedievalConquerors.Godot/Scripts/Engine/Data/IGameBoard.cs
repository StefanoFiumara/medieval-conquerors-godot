using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data;

public enum TileType
{
	Grass,
	Forest,
	Water
}

// NOTE: decide if Tile will need an interface + resource
public interface ITile
{
	TileType Type { get; }
}

public interface IGameBoard
{
	ITile GetTile(int x, int y, int z);
	IEnumerable<ITile> GetNeighbors(int x, int y, int z);
	IEnumerable<ITile> GetReachable(int x, int y, int z, int range);
	string GetMapStateHash();
}


