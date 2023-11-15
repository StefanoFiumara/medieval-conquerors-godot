using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

// TODO: Unit tests for this logic
public interface IGameBoard : IGameComponent
{
	ITileData GetTile(Vector2I pos);
	IEnumerable<ITileData> GetNeighbors(Vector2I pos);
	IEnumerable<ITileData> GetReachable(Vector2I pos, int range);
	IEnumerable<ITileData> SearchTiles(Func<ITileData, bool> predicate);
	int Distance(Vector2I start, Vector2I end);

	// TODO: Other functions we may want to implement:
	//			* Pathfinding
	//			* Field of View
	//			* https://www.redblobgames.com/grids/hexagons/
}

// NOTE: HexGameBoard logic assumes pointy-top hex tiles with Odd Offset coordinates
public class HexGameBoard : GameComponent, IGameBoard
{
	private readonly Dictionary<Vector2I, ITileData> _tiles;

	private static readonly Vector2I[] EvenHexDirections = 
	{
		new( 1,  0),
		new( 0, -1),
		new(-1, -1),
		new(-1,  0),
		new(-1,  1),
		new( 0,  1),
	};

	private static readonly Vector2I[] OddHexDirections =
	{
		new( 1,  0),
		new( 1, -1),
		new( 0, -1),
		new(-1,  0),
		new( 0,  1),
		new( 1,  1),
	};

	public HexGameBoard(Dictionary<Vector2I, ITileData> tileData)
	{
		_tiles = tileData;
	}
	
	public ITileData GetTile(Vector2I pos)
	{
		return _tiles.TryGetValue(pos, out var tile) ? tile : null;
	}

	public IEnumerable<ITileData> GetNeighbors(Vector2I pos)
	{
		var directions = pos.Y % 2 == 0 ? EvenHexDirections : OddHexDirections;

		foreach (var dir in directions)
		{
			var neighbor = pos + dir;
			if (_tiles.TryGetValue(neighbor, out var tile) && tile.IsWalkable)
			{
				yield return tile;
			}
		}
	}

	// TODO: Pathfinding algorithm instead?
	public IEnumerable<ITileData> GetReachable(Vector2I pos, int range)
	{
		var visited = new HashSet<Vector2I> { pos };
		var outerEdges = new List<List<Vector2I>> { new() { pos } };

		for (int i = 1; i <= range; i++)
		{
			outerEdges.Add(new List<Vector2I>());
			foreach (var position in outerEdges[i-1])
			{
				foreach (var neighbor in GetNeighbors(position))
				{
					if (!visited.Contains(neighbor.Position))
					{
						visited.Add(neighbor.Position);
						outerEdges[i].Add(neighbor.Position);
							
						yield return neighbor;
					}
				}
			}
		}
	}

	public IEnumerable<ITileData> SearchTiles(Func<ITileData, bool> predicate)
	{
		return _tiles.Values.Where(predicate);
	}

	public int Distance(Vector2I start, Vector2I end)
	{
		if (start.X == end.X)
		{
			return Mathf.Abs(end.Y - start.Y);
		}

		if (start.Y == end.Y)
		{
			return Mathf.Abs(end.X - start.X);
		}

		int dx = Mathf.Abs(end.X - start.X);
		int dy = Mathf.Abs(end.Y - start.Y);

		if (start.Y < end.Y)
		{
			return dx + dy - Mathf.CeilToInt(dx / 2.0);
		}

		return dx + dy - Mathf.FloorToInt(dx / 2.0);
	}
}


