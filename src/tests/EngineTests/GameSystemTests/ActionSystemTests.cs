using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Events;
using MedievalConquerors.Engine.GameComponents;
using Shouldly;

namespace MedievalConquerors.Tests.EngineTests.GameSystemTests;

internal class TestGameAction : GameAction { }

public class ActionSystemTests : GameSystemTestFixture
{
    private readonly ActionSystem _actionSystem;

    public ActionSystemTests(ITestOutputHelper output, CardLibraryFixture libraryFixture) : base(output, libraryFixture)
    {
        Game.Awake();
        _actionSystem = Game.GetComponent<ActionSystem>();
    }

    [Fact]
    public void GameFactory_Creates_ActionSystem() => _actionSystem.ShouldNotBeNull();

    [Fact]
    public void ActionSystem_OnPerform_System_Becomes_Active()
    {
        var action = new TestGameAction();
        Game.Perform(action);

        Assert.True(_actionSystem.IsActive);
    }

    [Fact]
    public void ActionSystem_OnUpdate_Raises_BeginSequenceNotification()
    {
        bool eventRaised = false;
        void BeginSequenceHandler() => eventRaised = true;

        Events.Subscribe(ActionSystem.BEGIN_ACTION_EVENT, BeginSequenceHandler);

        var action = new TestGameAction();
        Game.Perform(action);
        Game.Update();

        Assert.True(eventRaised);
    }

    [Fact]
    public void ActionSystem_OnUpdate_Validates_GameAction()
    {
        bool eventRaised = false;

        Events.Subscribe(GameEvent.Validate<TestGameAction>(), SetEventRaised);

        var action = new TestGameAction();
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

        Events.Subscribe<TestGameAction, ActionValidatorResult>(GameEvent.Validate<TestGameAction>(), InvalidateAction);
        Events.Subscribe(GameEvent.Cancel<TestGameAction>(), SetEventRaised);

        var action = new TestGameAction();
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

        Events.Subscribe(GameEvent.Prepare<TestGameAction>(), SetEventRaised);

        var action = new TestGameAction();
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

        Events.Subscribe(GameEvent.Perform<TestGameAction>(), SetEventRaised);

        var action = new TestGameAction();
        Game.Perform(action);
        Game.Update();

        Assert.True(eventRaised);
        return;

        void SetEventRaised() => eventRaised = true;
    }


    public static IEnumerable<object[]> ExpectedEvents => new List<object[]>
    {
        new object[]{ ActionSystem.BEGIN_ACTION_EVENT },
        new object[]{ GameEvent.Validate<TestGameAction>() },
        new object[]{ GameEvent.Prepare<TestGameAction>() },
        new object[]{ GameEvent.Perform<TestGameAction>() },
        new object[]{ ActionSystem.END_ACTION_EVENT },
        new object[]{ ActionSystem.COMPLETE_ACTION_EVENT },
    };

    [Theory]
    [MemberData(nameof(ExpectedEvents))]
    public void ActionSystem_OnUpdate_Raises_Event(string eventKey)
    {
        bool eventRaised = false;

        Events.Subscribe(eventKey, SetEventRaised);

        var action = new TestGameAction();
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

        void PerformEvent(TestGameAction sender)
        {
            senderId = sender.Id;
            eventRaised = true;
        }

        Events.Subscribe<TestGameAction>(GameEvent.Perform<TestGameAction>(), PerformEvent);

        var action1 = new TestGameAction();
        var action2 = new TestGameAction();

        Game.Perform(action1);
        Game.Perform(action2);

        Game.Update();

        Assert.True(eventRaised);
        Assert.NotEqual(Guid.Empty, senderId);

        Assert.Equal(action1.Id, senderId);
        Assert.NotEqual(action2.Id, senderId);
    }
}
