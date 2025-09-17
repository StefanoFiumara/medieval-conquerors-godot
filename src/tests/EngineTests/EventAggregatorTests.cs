using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Tests.EngineTests;

public class EventAggregatorTests
{
    private readonly EventAggregator _events = new();

    [Fact]
    public void EventAggregator_Subscribe_Fires_Event()
    {
        bool eventFired = false;
        void Handler() => eventFired = true;

        _events.Subscribe("fire", Handler);
        _events.Publish("fire");

        Assert.True(eventFired);
    }

    [Fact]
    public void EventAggregator_Subscribe_With_Sender_Fires_Event()
    {
        bool eventFired = false;
        void Handler(string sender)
        {
            eventFired = true;
            Assert.Equal("sender", sender);
        }

        _events.Subscribe<string>("fire", Handler);
        _events.Publish("fire", "sender");

        Assert.True(eventFired);
    }

    [Fact]
    public void EventAggregator_Subscribe_With_Sender_Args_Fires_Event()
    {
        bool eventFired = false;
        void Handler(string sender, string args)
        {
            eventFired = true;
            Assert.Equal("sender", sender);
            Assert.Equal("args", args);
        }

        _events.Subscribe<string, string>("fire", Handler);
        _events.Publish("fire", "sender", "args");

        Assert.True(eventFired);
    }

    [Fact]
    public void EventAggregator_Subscribe_And_Publish_With_Less_Args_Fires_Event()
    {
        bool eventFired = false;
        void Handler(string sender, string args)
        {
            eventFired = true;
            Assert.Equal("sender", sender);
            Assert.Null(args);
        }

        // NOTE: publishing with one less parameter than subscribed.
        _events.Subscribe<string, string>("fire", Handler);
        _events.Publish("fire", "sender");

        Assert.True(eventFired);
    }

    [Fact]
    public void EventAggregator_Subscribe_WithOneArg_And_Publish_WithTwoArgs_Fires_Event()
    {
        bool eventFired = false;
        void Handler(string sender)
        {
            eventFired = true;
            Assert.Equal("sender", sender);
        }

        // NOTE: publishing with one more parameter than subscribed.
        _events.Subscribe<string>("fire", Handler);
        _events.Publish("fire", "sender", "args");

        Assert.True(eventFired);
    }

    [Fact]
    public void EventAggregator_Subscribe_WithNoArgs_And_Publish_WithTwoArgs_Fires_Event()
    {
        bool eventFired = false;
        void Handler() => eventFired = true;

        // NOTE: publishing with one more parameter than subscribed.
        _events.Subscribe("fire", Handler);
        _events.Publish("fire", "sender", "args");

        Assert.True(eventFired);
    }

    [Fact]
    public void EventAggregator_Unsubscribe_Does_Not_Fire_Event()
    {
        bool eventFired = false;
        void Handler() => eventFired = true;

        _events.Subscribe("fire", Handler);
        _events.Publish("fire");
        Assert.True(eventFired);

        eventFired = false;

        _events.Unsubscribe("fire", Handler);
        _events.Publish("fire");

        Assert.False(eventFired);
    }

    [Fact]
    public void EventAggregator_Multiple_Subscribers_All_Fire_Event()
    {
        bool event1Fired = false;
        bool event2Fired = false;

        void Handler1() => event1Fired = true;
        void Handler2() => event2Fired = true;

        _events.Subscribe("fire", Handler1);
        _events.Subscribe("fire", Handler2);
        _events.Publish("fire");

        Assert.True(event1Fired);
        Assert.True(event2Fired);
    }
}
