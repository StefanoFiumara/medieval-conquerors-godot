using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests.Engine.GameSystemTests;

public class ActionSystemTests : GameSystemTestFixture
{
    private readonly ActionSystem _underTest;

    public ActionSystemTests(ITestOutputHelper output) : base(output)
    {
        _underTest = Game.GetComponent<ActionSystem>();
    }

    [Fact]
    public void GameFactory_Creates_ActionSystem()
    {
        Assert.NotNull(_underTest);
    }
    
    [Fact]
    public void ActionSystem_OnPerform_System_Becomes_Active()
    {
        var action = new GameAction();
        Game.Perform(action);
        
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
        
        Events.Subscribe(ActionSystem.BeginSequenceEvent, BeginSequenceHandler);
        
        var action = new GameAction();
        Game.Perform(action);
        Game.Update();
        
        Assert.True(eventRaised);
    }
    
    [Fact]
    public void ActionSystem_OnUpdate_Validates_GameAction()
    {
        bool eventRaised = false;

        Events.Subscribe(GameEvent.Validate<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        Game.Perform(action);
        Game.Update();
        
        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }
    
    [Fact]
    public void ActionSystem_OnUpdate_Invalid_GameAction_Is_Canceled()
    {
        bool eventRaised = false;

        Events.Subscribe<GameAction, ActionValidatorResult>(GameEvent.Validate<GameAction>(), InvalidateAction);
        Events.Subscribe(GameEvent.Cancel<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        Game.Perform(action);
        Game.Update();
        
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
        
        Events.Subscribe(GameEvent.Prepare<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        Game.Perform(action);
        Game.Update();
        
        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }
    
    [Fact]
    public void ActionSystem_OnUpdate_Performs_Action()
    {
        bool eventRaised = false;
        
        Events.Subscribe(GameEvent.Perform<GameAction>(), SetEventRaised);
        
        var action = new GameAction();
        Game.Perform(action);
        Game.Update();
        
        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }


    public static IEnumerable<object[]> ExpectedEvents => new List<object[]>
    {
        new object[]{ ActionSystem.BeginSequenceEvent },
        new object[]{ GameEvent.Validate<GameAction>() },
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
        
        Events.Subscribe(eventKey, SetEventRaised);
        
        var action = new GameAction();
        Game.Perform(action);
        Game.Update();
        
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

        Events.Subscribe<GameAction>(GameEvent.Perform<GameAction>(), PerformEvent);
        
        var action1 = new GameAction();
        var action2 = new GameAction();
        
        Game.Perform(action1);
        Game.Perform(action2);
        
        Game.Update();
        
        Assert.True(eventRaised);
        Assert.NotEqual(Guid.Empty, senderId);
        
        Assert.Equal(action1.Id, senderId);
        Assert.NotEqual(action2.Id, senderId);
    }
}