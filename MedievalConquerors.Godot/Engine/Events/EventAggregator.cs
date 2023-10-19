﻿using System;
using System.Collections.Generic;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.Events;

public interface IEventAggregator : IGameComponent
{
    void Subscribe(string eventName, Action handler);
    void Subscribe<TSender>(string eventName, Action<TSender> handler)
        where TSender : class;
    void Subscribe<TSender, TArgs>(string eventName, Action<TSender, TArgs> handler)
        where TSender : class
        where TArgs : class;
    void Unsubscribe(string eventName, Delegate handler);
    void Publish(string eventName);

    void Publish<TSender>(string eventName, TSender sender)
        where TSender : class;
    
    void Publish<TSender, TArgs>(string eventName, TSender sender, TArgs args)
        where TSender : class
        where TArgs : class;
}

public class EventAggregator : GameComponent, IEventAggregator, IAwake, IDestroy
{
    private readonly Dictionary<string, IList<Delegate>> _events = new();

    private ILogger _logger = new NullLogger();
    
    public void Awake()
    {
        _logger = Game.GetComponent<ILogger>();
    }

    public void Destroy()
    {
        _events.Clear();
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
        _logger.Info($"Subscribed <{eventName}> :: <{handler.Method.DeclaringType?.Name}.{handler.Method.Name}>");
        if (_events.ContainsKey(eventName) == false)
        {
            _events[eventName] = new List<Delegate> { handler };
        }
        else
        {
            _events[eventName].Add(handler);
        }
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
        _logger.Info($"Published <{eventName}>");
        if (!_events.TryGetValue(eventName, out var actions) || (actions.Count == 0))
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
                _ => default
            };
        }
    }
    
    public void Unsubscribe(string eventName, Delegate handler)
    {
        if (!_events.ContainsKey(eventName)) return;

        _logger.Info($"Unsubscribed <{eventName} :: {handler.Method.DeclaringType?.Name}.{handler.Method.Name}>");
        while (_events[eventName].Contains(handler))
        {
            _events[eventName].Remove(handler);
        }
    }
}