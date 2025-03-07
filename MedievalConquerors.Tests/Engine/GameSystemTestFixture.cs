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
    private readonly IGameSettings _settings = Substitute.For<IGameSettings>();
    protected readonly HexMap Map = CreateMockGameMap();

    protected readonly IGame Game;
    protected readonly EventAggregator Events;

    protected GameSystemTestFixture(ITestOutputHelper output, CardLibraryFixture libraryFixture)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        Fixture = new Fixture();

        _settings.DebugMode.Returns(true);

        _settings.StartingFoodCount.Returns(5);
        _settings.StartingWoodCount.Returns(5);
        _settings.StartingGoldCount.Returns(2);
        _settings.StartingStoneCount.Returns(0);

        Game = GameFactory.Create(logger, Map, _settings, libraryFixture.Library);
        Events = Game.GetComponent<EventAggregator>();
    }

    [Fact]
    public void Game_Destroy_Unsubscribes_All_Events()
    {
        Game.Awake();
        Events.Subscriptions.ShouldNotBeEmpty();
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
