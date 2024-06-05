using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

public interface IGameMap : IGameComponent
{
	ITileData GetTile(Vector2I pos);
	IEnumerable<Vector2I> GetNeighbors(Vector2I pos);
	IEnumerable<Vector2I> GetReachable(Vector2I pos, int range);
	List<ITileData> SearchTiles(Func<ITileData, bool> predicate);
	int Distance(Vector2I start, Vector2I end);
	List<Vector2I> CalculatePath(Vector2I start, Vector2I end);

	// TODO: Other functions we may want to implement:
	//			* Field of View
	//			* https://www.redblobgames.com/grids/hexagons/
}

// NOTE: HexMap logic assumes pointy-top hex tiles with Odd Offset coordinates
// TODO: Unit tests for this logic
public class HexMap : GameComponent, IGameMap
{
	public static readonly Vector2I None = new(int.MinValue, int.MinValue);
	
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

	public HexMap(Dictionary<Vector2I, ITileData> tileData)
	{
		_tiles = tileData;
	}
	
	public ITileData GetTile(Vector2I pos)
	{
		return _tiles.GetValueOrDefault(pos);
	}

	public IEnumerable<Vector2I> GetNeighbors(Vector2I pos)
	{
		var directions = pos.Y % 2 == 0 ? EvenHexDirections : OddHexDirections;

		foreach (var dir in directions)
		{
			var neighbor = pos + dir;
			if (_tiles.TryGetValue(neighbor, out var tile) && tile.IsWalkable)
			{
				yield return tile.Position;
			}
		}
	}

	// TODO: Pathfinding algorithm instead?
	public IEnumerable<Vector2I> GetReachable(Vector2I pos, int range)
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
					if (visited.Add(neighbor))
					{
						outerEdges[i].Add(neighbor);
						yield return neighbor;
					}
				}
			}
		}
	}

	public List<ITileData> SearchTiles(Func<ITileData, bool> predicate)
	{
		return _tiles.Values.Where(predicate).ToList();
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
	
	public List<Vector2I> CalculatePath(Vector2I start, Vector2I end)
	{
		var frontier = new PriorityQueue<Vector2I, int>();
		frontier.Enqueue(start, 0);

		var cameFrom = new Dictionary<Vector2I, Vector2I>
		{
			[start] = None
		};

		var costSoFar = new Dictionary<Vector2I, int>
		{
			[start] = 0
		};

		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			if (current == end)
				break;
			
			foreach (var neighbor in GetNeighbors(current))
			{
				var newCost = costSoFar[current] + 1;
				if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
				{
					costSoFar[neighbor] = newCost;
					var priority = newCost + Distance(end, neighbor);
					frontier.Enqueue(neighbor, priority);
					cameFrom[neighbor] = current;
				}
			}
		}
		
		// form the path
		var path = new List<Vector2I>();
		var step = end;
		while(step != start)
		{
			path.Add(step);
			step = cameFrom[step];
		}

		path.Reverse();
		return path;
	}
}


