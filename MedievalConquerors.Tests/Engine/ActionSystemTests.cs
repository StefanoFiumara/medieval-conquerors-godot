using MedievalConquerors.Engine;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using MedievalConquerors.Engine.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine;

public class ActionSystemTests
{
    private readonly ActionSystem _underTest;

    private readonly IGameSettings _settings = Substitute.For<IGameSettings>();
    private readonly IMatch _match = Substitute.For<IMatch>();
    private readonly IGameBoard _board = Substitute.For<IGameBoard>();
    
    private readonly IGame _game;
    private readonly IEventAggregator _events;

    public ActionSystemTests(ITestOutputHelper output)
    {
        var logger = new TestLogger(output, LogLevel.Info);
        
        _game = GameFactory.Create(logger, _match, _board, _settings);
        _underTest = _game.GetComponent<ActionSystem>();
        _events = _game.GetComponent<EventAggregator>();
        
        _game.Awake();
    }

    [Fact]
    public void ActionSystem_OnPerform_System_Becomes_Active()
    {
        var action = new GameAction();
        _game.Perform(action);
        
        Assert.True(_underTest.IsActive);
    }

    [Fact]
    public void ActionSystem_OnUpdate_Raises_BeginSequenceNotification()
    {
        bool eventRaised = false;
        void BeginSequenceHandler()
        {
            eventRaised = true;
        }
        
        _events.Subscribe(ActionSystem.BeginSequenceEvent, BeginSequenceHandler);
        
        var action = new GameAction();
        _game.Perform(action);
        _game.Update();
        
        Assert.True(eventRaised);
    }
    
    [Fact]
    public void ActionSystem_OnUpdate_Validates_GameAction()
    {
        bool eventRaised = false;

        _events.Subscribe(GameEvent.Validate<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        _game.Perform(action);
        _game.Update();
        
        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }
    
    [Fact]
    public void ActionSystem_OnUpdate_Invalid_GameAction_Is_Canceled()
    {
        bool eventRaised = false;

        _events.Subscribe<GameAction, ActionValidatorResult>(GameEvent.Validate<GameAction>(), InvalidateAction);
        _events.Subscribe(GameEvent.Cancel<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        _game.Perform(action);
        _game.Update();
        
        Assert.True(eventRaised);
        return;

        void InvalidateAction(GameAction _, ActionValidatorResult validator) 
            => validator.Invalidate("Invalid Action");

        void SetEventRaised() => eventRaised = true;
    }
    
    [Fact]
    public void ActionSystem_OnUpdate_Prepares_Action()
    {
        bool eventRaised = false;
        
        _events.Subscribe(GameEvent.Prepare<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        _game.Perform(action);
        _game.Update();
        
        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }
    
    [Fact]
    public void ActionSystem_OnUpdate_Performs_Action()
    {
        bool eventRaised = false;
        
        _events.Subscribe(GameEvent.Perform<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        _game.Perform(action);
        _game.Update();
        
        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }


    public static IEnumerable<object[]> ExpectedEvents => new List<object[]>
    {
        new object[]{ ActionSystem.BeginSequenceEvent },
        new object[]{  GameEvent.Validate<GameAction>() },
        new object[]{ GameEvent.Prepare<GameAction>() },
        new object[]{ GameEvent.Perform<GameAction>() },
        new object[]{ ActionSystem.EndSequenceEvent },
        new object[]{ ActionSystem.CompleteEvent },
    };
    
    [Theory]
    [MemberData(nameof(ExpectedEvents))]
    public void ActionSystem_OnUpdate_Raises_Event(string eventKey)
    {
        bool eventRaised = false;
        
        _events.Subscribe(eventKey, SetEventRaised);
        
        var action = new GameAction();
        _game.Perform(action);
        _game.Update();
        
        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }

    [Fact]
    public void ActionSystem_Cannot_Perform_New_GameAction_When_Already_Active()
    {
        bool eventRaised = false;
        var senderId = Guid.Empty;
        
        void PerformEvent(GameAction sender)
        {
            senderId = sender.Id;
            eventRaised = true;
        }

        _events.Subscribe<GameAction>(GameEvent.Perform<GameAction>(), PerformEvent);
        
        var action1 = new GameAction();
        var action2 = new GameAction();
        
        _game.Perform(action1);
        _game.Perform(action2);
        
        _game.Update();
        
        Assert.True(eventRaised);
        Assert.NotEqual(Guid.Empty, senderId);
        
        Assert.Equal(action1.Id, senderId);
        Assert.NotEqual(action2.Id, senderId);
    }
    
    // TODO: Figure out how to unit test reactions
    // TODO: Figure out if we can unit test viewers?
}