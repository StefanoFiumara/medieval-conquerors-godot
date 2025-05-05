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
	public const string TerrainType = "TerrainType";
	public const string ResourceType = "ResourceType";
	public const string ResourceYield = "ResourceYield";
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
			// TODO: Resource yields should probably go somewhere else so that they are not tied to the tile set and are easier to edit (Game Settings?)
			// TODO: Resource type for the yield should be derived from the terrain type, rather than stored as separate data.
			var terrain = tileMap.GetCellTileData(pos).GetCustomData(CustomTileData.TerrainType).As<TileTerrain>();
			var resourceType = tileMap.GetCellTileData(pos).GetCustomData(CustomTileData.ResourceType).As<ResourceType>();
			var resourceYield = tileMap.GetCellTileData(pos).GetCustomData(CustomTileData.ResourceYield).As<int>();

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
