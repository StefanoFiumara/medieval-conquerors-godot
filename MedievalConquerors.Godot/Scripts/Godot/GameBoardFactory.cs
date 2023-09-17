using System.Collections.Generic;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;
using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Godot;

public static class GameBoardFactory
{
    // Create a game board using a Godot TileMap
    // NOTE: TileMap must be configured with Pointy-top hex tiles and Odd Offset Coordinates
    public static IGameBoard Create(TileMap tileMap)
    {
        var tileData = CreateTileData(tileMap);
        return new HexGameBoard(tileData);
    }
	
    // NOTE: This maps the atlas coord of each tile in the tileset to a tile terrain type
    // TODO: Can we find a better and more scalable solution to this?
    private static readonly Dictionary<Vector2I, TileTerrain> TileTerrainMap = new()
    {
        { new(0, 0), TileTerrain.Water },
        { new(1, 0), TileTerrain.Grass },
        { new(2, 0), TileTerrain.Forest },
        { new(3, 0), TileTerrain.DeepWater },
        { new(4, 0), TileTerrain.TownCenter },
        { new(0, 1), TileTerrain.ShoreFish },
        { new(1, 1), TileTerrain.Berries },
        { new(2, 1), TileTerrain.Deer },
        { new(3, 1), TileTerrain.Gold },
        { new(4, 1), TileTerrain.Stone },
    };
	
    private static Dictionary<Vector2I, ITileData> CreateTileData(TileMap tileMap)
    {
        var tiles = new  Dictionary<Vector2I, ITileData>();
        var cells = tileMap.GetUsedCells(0).ToList();
		
        foreach (var pos in cells)
        {
            var atlasCoords = tileMap.GetCellAtlasCoords(0, pos);
			
            if (TileTerrainMap.TryGetValue(atlasCoords, out var terrain))
            {
                var tile = new TileData(pos, terrain);
                tiles.Add(pos, tile);
            }
            else
            {
                GD.PrintErr($"Could not find terrain mapping for tile at atlas coord {atlasCoords} in TileSet");
            }
        }

        return tiles;
    }
}