using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine;

public abstract class GameSystemTestFixture
{
    protected readonly IGameSettings Settings = Substitute.For<IGameSettings>();
    protected readonly IMatch Match = Substitute.For<IMatch>();
    protected readonly IGameBoard Board = Substitute.For<IGameBoard>();
    
    protected readonly IGame Game;
    protected readonly IEventAggregator Events;

    protected GameSystemTestFixture(ITestOutputHelper output)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        
        Game = GameFactory.Create(logger, Match, Board, Settings);
        
        Events = Game.GetComponent<EventAggregator>();
        
        Game.Awake();
    }
}