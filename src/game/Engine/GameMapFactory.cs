using System;
using System.Collections.Generic;
using System.Linq;
using Fano.ASCIITableUtil;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Extensions;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Engine;

public static class CustomTileData
{
	public const string TERRAIN_TYPE = "TerrainType";
}

public static class GameMapFactory
{
	// Create a map using a Godot TileMap
	// NOTE: TileMap must be configured with Pointy-top hex tiles and Odd Offset Coordinates
	public static HexMap CreateHexMap(TileMapLayer tileMap)
	{
		var (tileData, terrainAtlasMap) = CreateTileData(tileMap);
		return new HexMap(tileData, terrainAtlasMap);
	}

	private static (Dictionary<Vector2I, TileData>, Dictionary<TileTerrain, Vector2I> terrainAtlasMap) CreateTileData(TileMapLayer tileMap)
	{
		var tiles = new  Dictionary<Vector2I, TileData>();
		var cells = tileMap.GetUsedCells().ToList();
		var terrainAtlasMap = new Dictionary<TileTerrain, Vector2I>();

		foreach (var pos in cells)
		{
			var terrain = tileMap.GetCellTileData(pos).GetCustomData(CustomTileData.TERRAIN_TYPE).As<TileTerrain>();
			var resourceType = terrain switch
			{
				TileTerrain.Forest => ResourceType.Wood,
				TileTerrain.Berries => ResourceType.Food,
				TileTerrain.Deer => ResourceType.Food,
				TileTerrain.Gold => ResourceType.Gold,
				TileTerrain.Stone => ResourceType.Stone,
				_ => ResourceType.None
			};

			// TODO: Make resource yields configurable (from game settings)
			var resourceYield = terrain switch
			{
				TileTerrain.Forest => 10,
				TileTerrain.Berries => 10,
				TileTerrain.Deer => 15,
				TileTerrain.Gold => 10,
				TileTerrain.Stone => 10,
				_ => 0
			};

			var tile = new TileData(pos, terrain, resourceType, resourceYield);
			tiles.Add(pos, tile);

			terrainAtlasMap.TryAdd(terrain, tileMap.GetCellAtlasCoords(pos));
		}

		var yieldData = tiles.Values
			.GroupBy(t => (t.Terrain, t.ResourceType, t.ResourceYield))
			.ToDictionary(g => g.Key, g => g.Count())
			.Select(kvp => new { kvp.Key.Terrain, TileCount = kvp.Value, kvp.Key.ResourceType, kvp.Key.ResourceYield })
			.ToList();

		GD.PrintRich("=== Parsed TileMap Data ===".Orange());
		var table = AsciiTable.Create(yieldData);
		GD.PrintRich($"{table}".Orange());
		return (tiles, terrainAtlasMap);
	}
}
