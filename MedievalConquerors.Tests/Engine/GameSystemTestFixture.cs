using AutoFixture;
using FluentAssertions;
using Godot;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using NSubstitute;
using Xunit.Abstractions;
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

    protected GameSystemTestFixture(ITestOutputHelper output)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        Fixture = new Fixture();

        Settings.StartingHandCount.Returns(5);
        Settings.DebugMode.Returns(false);
        
        Game = GameFactory.Create(logger, Map, Settings);
        Events = Game.GetComponent<EventAggregator>();
        
        Match = Game.GetComponent<Match>();
        Match.LocalPlayer.Resources[ResourceType.Food] = 4;
        Match.LocalPlayer.Resources[ResourceType.Gold] = 2;
        
        Game.Awake();
    }

    [Fact]
    public void Game_Destroy_Unsubscribes_All_Events()
    {
        Game.Destroy();
        Events.Subscriptions.Should().BeEmpty();
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
        tileData[townCenter1Pos] = new TileData(townCenter1Pos, TileTerrain.TownCenter, ResourceType.None, 0);
        tileData[townCenter2Pos] = new TileData(townCenter2Pos, TileTerrain.TownCenter, ResourceType.None, 0);
        
        return new HexMap(tileData, new());
    }
}