﻿using System;
using System.Collections.Generic;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.Events;

public class EventAggregator : GameComponent, IAwake, IDestroy
{
    private readonly Dictionary<string, IList<Delegate>> _subscriptions = new();
    private ILogger _logger = new NullLogger();

    public IReadOnlyDictionary<string, IList<Delegate>> Subscriptions => new Dictionary<string, IList<Delegate>>(_subscriptions);

    public void Awake()
    {
        _logger = Game.GetComponent<ILogger>();
    }

    public void Destroy()
    {
        _subscriptions.Clear();
    }

    public void Subscribe(string eventName, Action handler)
        => SubscribeInternal(eventName, handler);

    public void Subscribe<TSender>(string eventName, Action<TSender> handler)
        where TSender : class
        => SubscribeInternal(eventName, handler);

    public void Subscribe<TSender, TArgs>(string eventName, Action<TSender, TArgs> handler)
        where TSender : class
        where TArgs : class
        => SubscribeInternal(eventName, handler);

    private void SubscribeInternal(string eventName, Delegate handler)
    {
        _logger.Debug($"Subscribed <{eventName}> :: <{handler.Method.DeclaringType?.Name}.{handler.Method.Name}>");

        if (_subscriptions.TryGetValue(eventName, out var subscription))
            subscription.Add(handler);
        else
            _subscriptions[eventName] = new List<Delegate> { handler };
    }

    public void Publish(string eventName)
        => PublishInternal<object, object>(eventName, sender: null, args: null);

    public void Publish<TSender>(string eventName, TSender sender)
        where TSender : class
        => PublishInternal<TSender, object>(eventName, sender, args: null);

    public void Publish<TSender, TArgs>(string eventName, TSender sender, TArgs args)
        where TSender : class
        where TArgs : class
        => PublishInternal(eventName, sender, args);

    private void PublishInternal<TSender, TArgs>(string eventName, TSender sender, TArgs args)
    {
        _logger.Debug($"Published <{eventName}>");
        if (!_subscriptions.TryGetValue(eventName, out var actions) || (actions.Count == 0))
        {
            _logger.Debug($"\t* No Subscribers");
            return;
        }

        foreach (var action in actions)
        {
            _logger.Debug($"\t* Invoked <{action.Method.DeclaringType?.Name}.{action.Method.Name}>");
            _ = action.Method.GetParameters().Length switch
            {
                0 => action.DynamicInvoke(),
                1 => action.DynamicInvoke(sender),
                2 => action.DynamicInvoke(sender, args),
                _ => null
            };
        }
    }

    public void Unsubscribe(string eventName, Delegate handler)
    {
        if (!_subscriptions.ContainsKey(eventName)) return;

        _logger.Debug($"Unsubscribed <{eventName} :: {handler.Method.DeclaringType?.Name}.{handler.Method.Name}>");
        while (_subscriptions[eventName].Contains(handler))
        {
            _subscriptions[eventName].Remove(handler);
        }
    }
}
