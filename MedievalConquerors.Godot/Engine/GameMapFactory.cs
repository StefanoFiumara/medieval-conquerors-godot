using System.Collections.Generic;
using System.Linq;
using Fano.ASCIITableUtil;
using Godot;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Extensions;
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
    public static HexMap CreateHexMap(TileMap tileMap)
    {
        var tileData = CreateTileData(tileMap);
        return new HexMap(tileData);
    }
	
    private static Dictionary<Vector2I, TileData> CreateTileData(TileMap tileMap)
    {
        var tiles = new  Dictionary<Vector2I, TileData>();
        var cells = tileMap.GetUsedCells(0).ToList();
		
        foreach (var pos in cells)
        {
            var terrain = tileMap.GetCellTileData(0, pos).GetCustomData(CustomTileData.TerrainType).As<TileTerrain>();
            var resourceType = tileMap.GetCellTileData(0, pos).GetCustomData(CustomTileData.ResourceType).As<ResourceType>();
            var resourceYield = tileMap.GetCellTileData(0, pos).GetCustomData(CustomTileData.ResourceYield).As<int>();
			
            var tile = new TileData(pos, terrain, resourceType, resourceYield);
            tiles.Add(pos, tile);
        }
        
        var yieldData = tiles.Values
            .GroupBy(t => (t.Terrain, t.ResourceType, t.ResourceYield))
            .ToDictionary(g => g.Key, g => g.Count())
            .Select(kvp => new {kvp.Key.Terrain, TileCount = kvp.Value, kvp.Key.ResourceType, kvp.Key.ResourceYield })
            .ToList();
        
        GD.PrintRich("=== Parsed TileMap Data ===".Orange());
        var table = AsciiTable.Create(yieldData);
        GD.PrintRich($"{table}".Orange());
        return tiles;
    }
}
