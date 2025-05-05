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
public class HexMap(Dictionary<Vector2I, TileData> tileData, Dictionary<TileTerrain, Vector2I> terrainToAtlasCoordMap) : GameComponent
{
	public static readonly Vector2I None = new(int.MinValue, int.MinValue);

	/// <summary>
	/// Invoked when a tile is changed via SetTile, so that views can respond and update the corresponding TileMapLayer
	/// </summary>
	public delegate void TileChangedHandler(TileData oldTile, TileData newTile);
	public event TileChangedHandler OnTileChanged;

	private static readonly Vector2I[] EvenHexDirections =
	[
		new( 1,  0),
		new( 0, -1),
		new(-1, -1),
		new(-1,  0),
		new(-1,  1),
		new( 0,  1)
	];

	private static readonly Vector2I[] OddHexDirections =
	[
		new( 1,  0),
		new( 1, -1),
		new( 0, -1),
		new(-1,  0),
		new( 0,  1),
		new( 1,  1)
	];

	public Vector2I GetAtlasCoord(TileTerrain terrain) => terrainToAtlasCoordMap.GetValueOrDefault(terrain, None);

	public TileData GetTile(Vector2I pos) => tileData.GetValueOrDefault(pos);

	public void SetTile(Vector2I pos, TileTerrain terrain, ResourceType resource = ResourceType.None, int resourceYield = 0)
	{
		if (tileData.ContainsKey(pos))
		{
			var oldTileData = tileData[pos];
			tileData[pos] = new TileData(pos, terrain, resource, resourceYield);
			OnTileChanged?.Invoke(oldTileData, tileData[pos]);
		}
	}

	public IEnumerable<TileData> GetNeighbors(Vector2I pos)
	{
		var directions = pos.Y % 2 == 0 ? EvenHexDirections : OddHexDirections;

		foreach (var dir in directions)
		{
			var neighbor = pos + dir;
			if (tileData.TryGetValue(neighbor, out var tile))
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
			outerEdges.Add([]);
			foreach (var edge in outerEdges[i-1])
			{
				foreach (var neighbor in GetNeighbors(edge))
				{
					var position = neighbor.Position;
					if (visited.Add(position))
					{
						outerEdges[i].Add(position);
						if(neighbor.IsWalkable)
							yield return position;
					}
				}
			}
		}
	}

	public List<TileData> SearchTiles(Func<TileData, bool> predicate)
	{
		return tileData.Values.Where(predicate).ToList();
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

			foreach (var neighbor in GetNeighbors(current).Where(n => n.IsWalkable || n.Position == end))
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

	private static int Distance(Vector2I start, Vector2I end)
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
