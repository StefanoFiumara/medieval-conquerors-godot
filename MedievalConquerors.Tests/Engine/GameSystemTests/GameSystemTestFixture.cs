using AutoFixture;
using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public abstract class GameSystemTestFixture
{
    protected readonly Fixture Fixture;
    protected readonly IGameSettings Settings = Substitute.For<IGameSettings>();
    protected readonly IGameBoard Board = Substitute.For<IGameBoard>();
    
    protected readonly IGame Game;
    protected readonly IEventAggregator Events;

    protected GameSystemTestFixture(ITestOutputHelper output)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        
        Fixture = new Fixture();
        Fixture.Inject(Substitute.For<ICardData>());
        
        Game = GameFactory.Create(logger, Board, Settings);
        Events = Game.GetComponent<EventAggregator>();
        Game.Awake();
    }
}