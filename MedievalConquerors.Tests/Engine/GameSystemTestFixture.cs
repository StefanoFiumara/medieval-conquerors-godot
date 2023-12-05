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
    protected readonly IGameBoard Board = CreateMockGameBoard();

    protected readonly IGame Game;
    protected readonly IEventAggregator Events;

    protected GameSystemTestFixture(ITestOutputHelper output)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        Fixture = new Fixture();
        
        Fixture.Register(CreateCardData);
        Game = GameFactory.Create(logger, Board, Settings);
        
        Events = Game.GetComponent<EventAggregator>();
        var match = Game.GetComponent<Match>();
        
        SetupPlayer(match.LocalPlayer);
        SetupPlayer(match.EnemyPlayer);
        
        Game.Awake();
    }

    [Fact]
    public void Game_Destroy_Unsubscribes_All_Events()
    {
        Game.Destroy();
        Events.Subscriptions.Should().BeEmpty();
    }

    private ICardData CreateCardData()
    {
        var substitute = Substitute.For<ICardData>();
        substitute.Title.Returns(Fixture.Create<string>());
        substitute.Description.Returns(Fixture.Create<string>());
        substitute.Attributes.Returns(new List<ICardAttribute>());
        return substitute;
    }

    private void SetupPlayer(IPlayer player)
    {
        var dummyCards = Fixture.Build<Card>()
            .FromFactory((ICardData data) => new Card(data, player, Zone.Deck))
            .Without(c => c.BoardPosition)
            .CreateMany(30);
        
        player.Deck.AddRange(dummyCards);
    }

    private static IGameBoard CreateMockGameBoard()
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

        return new HexGameBoard(tileData);
    }
}