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
    protected readonly IMap Map = CreateMockGameMap();

    protected readonly IGame Game;
    protected readonly IEventAggregator Events;

    protected GameSystemTestFixture(ITestOutputHelper output)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        Fixture = new Fixture();
        
        Game = GameFactory.Create(logger, Map, Settings);
        
        Events = Game.GetComponent<EventAggregator>();
        Game.GetComponent<Match>().LocalPlayer.Resources.Food = 4;
        Game.GetComponent<Match>().LocalPlayer.Resources.Gold = 2;
        Game.Awake();
    }

    [Fact]
    public void Game_Destroy_Unsubscribes_All_Events()
    {
        Game.Destroy();
        Events.Subscriptions.Should().BeEmpty();
    }

    private static IMap CreateMockGameMap()
    {
        var tileData = new Dictionary<Vector2I, ITileData>();
        
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                var pos = new Vector2I(x, y);
                tileData.Add(pos, new TileData(pos, TileTerrain.Grass, ResourceType.None, 0));
            }
        }

        return new HexMap(tileData);
    }
}