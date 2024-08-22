using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Data;

// TODO: Unit tests for this logic
// TODO: Other functions we may want to implement:
//			* Field of View
//			* https://www.redblobgames.com/grids/hexagons/
// NOTE: HexMap logic assumes pointy-top hex tiles with Odd Offset coordinates
public class HexMap : GameComponent
{
	public static readonly Vector2I None = new(int.MinValue, int.MinValue);
	
	private readonly Dictionary<Vector2I, TileData> _tiles;
	private readonly Dictionary<TileTerrain, Vector2I> _terrainToAtlasCoordMap;

	public event Action<TileData> OnTileChanged; 

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

	public HexMap(Dictionary<Vector2I, TileData> tileData, Dictionary<TileTerrain, Vector2I> terrainToAtlasCoordMap)
	{
		_tiles = tileData;
		_terrainToAtlasCoordMap = terrainToAtlasCoordMap;
	}

	public Vector2I GetAtlasCoord(TileTerrain terrain) => _terrainToAtlasCoordMap.ContainsKey(terrain) ? _terrainToAtlasCoordMap[terrain] : None;
	
	public TileData GetTile(Vector2I pos)
	{
		return _tiles.GetValueOrDefault(pos);
	}

	public void SetTile(Vector2I pos, TileTerrain terrain, ResourceType resource = ResourceType.None, int resourceYield = 0)
	{
		if (_tiles.ContainsKey(pos))
		{
			_tiles[pos] = new TileData(pos, terrain, resource, resourceYield);
			OnTileChanged?.Invoke(_tiles[pos]);
		}
	}

	public IEnumerable<TileData> GetNeighbors(Vector2I pos)
	{
		var directions = pos.Y % 2 == 0 ? EvenHexDirections : OddHexDirections;

		foreach (var dir in directions)
		{
			var neighbor = pos + dir;
			if (_tiles.TryGetValue(neighbor, out var tile))
			{
				yield return tile;
			}
		}
	}
	
	public IEnumerable<Vector2I> GetReachable(Vector2I pos, int range)
	{
		var visited = new HashSet<Vector2I> { pos };
		var outerEdges = new List<List<Vector2I>> { new() { pos } };

		if(GetTile(pos).IsWalkable)
			yield return pos;
		
		for (int i = 1; i <= range; i++)
		{
			outerEdges.Add(new List<Vector2I>());
			foreach (var edge in outerEdges[i-1])
			{
				foreach (var neighbor in GetNeighbors(edge).Where(n => n.IsWalkable))
				{
					var position = neighbor.Position;
					if (visited.Add(position))
					{
						outerEdges[i].Add(position);
						yield return position;
					}
				}
			}
		}
	}

	public List<TileData> SearchTiles(Func<TileData, bool> predicate)
	{
		return _tiles.Values.Where(predicate).ToList();
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
			
			foreach (var neighbor in GetNeighbors(current).Where(n => n.IsWalkable))
			{
				var position = neighbor.Position;
				var newCost = costSoFar[current] + 1;
				if (!costSoFar.ContainsKey(position) || newCost < costSoFar[position])
				{
					costSoFar[position] = newCost;
					var priority = newCost + Distance(end, position);
					frontier.Enqueue(position, priority);
					cameFrom[position] = current;
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

	private int Distance(Vector2I start, Vector2I end)
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


