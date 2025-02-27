using AutoFixture;
using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using NSubstitute;
using Shouldly;

using TileData = MedievalConquerors.Engine.Data.TileData;

namespace MedievalConquerors.Tests.Engine;

public abstract class GameSystemTestFixture
{
    protected readonly Fixture Fixture;
    protected readonly IGameSettings Settings = Substitute.For<IGameSettings>();
    protected readonly HexMap Map = CreateMockGameMap();

    protected readonly IGame Game;
    protected readonly IEventAggregator Events;
    protected readonly Match Match;

    protected GameSystemTestFixture(ITestOutputHelper output, CardLibraryFixture libraryFixture)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        Fixture = new Fixture();

        Settings.StartingHandCount.Returns(5);
        Settings.DebugMode.Returns(true);
        
        Settings.StartingFoodCount.Returns(5);
        Settings.StartingWoodCount.Returns(5);
        Settings.StartingGoldCount.Returns(2);
        Settings.StartingStoneCount.Returns(0);
        
        Game = GameFactory.Create(logger, Map, Settings, libraryFixture.Library);
        Events = Game.GetComponent<EventAggregator>();
        
        Match = Game.GetComponent<Match>();
        Match.LocalPlayer.Resources[ResourceType.Food] = 4;
        Match.LocalPlayer.Resources[ResourceType.Gold] = 2;
        
        // TODO: Since debug mode is enabled, we need to populate the player decks manually for each unit test
        // Should we do that in each system test separately, before calling awake?
        // This way each system can test a very specific set of cards.
        Game.Awake();
    }

    [Fact]
    public void Game_Destroy_Unsubscribes_All_Events()
    {
        Game.Destroy();
        Events.Subscriptions.ShouldBeEmpty();
    }

    private static HexMap CreateMockGameMap()
    {
        var tileData = new Dictionary<Vector2I, TileData>();
        
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                var pos = new Vector2I(x, y);
                tileData.Add(pos, new TileData(pos, TileTerrain.Grass, ResourceType.None, 0));
            }
        }

        var townCenter1Pos = new Vector2I(4, 4);
        var townCenter2Pos = new Vector2I(6, 6);
        tileData[townCenter1Pos] = new TileData(townCenter1Pos, TileTerrain.StartingTownCenterBlue, ResourceType.None, 0);
        tileData[townCenter2Pos] = new TileData(townCenter2Pos, TileTerrain.StartingTownCenterRed, ResourceType.None, 0);
        
        return new HexMap(tileData, new());
    }
}