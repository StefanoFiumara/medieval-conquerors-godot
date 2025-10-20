using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace MedievalConquerors.Engine.Data;

public class Card
{
    private readonly ImmutableDictionary<Type, ICardAttribute> _attributeMap;
    private readonly Dictionary<Type, List<Delegate>> _modifiers = new();

    public CardData Data { get; }

    public Player Owner { get; }
    public Zone Zone { get; set; }
    public Vector2I MapPosition { get; set; }

    public Card(CardData data, Player owner, Zone zone = Zone.None, Vector2I mapPosition = default)
    {
        Data = data;
        Owner = owner;
        Zone = zone;
        MapPosition = Zone == Zone.Map ? mapPosition : HexMap.None;
        _attributeMap = Data.Attributes.ToImmutableDictionary(attr => attr.GetType(), attr => attr);
    }

    public bool HasAttribute<TAttribute>() where TAttribute : class, ICardAttribute => HasAttribute<TAttribute>(out _);
    public bool HasAttribute<TAttribute>(out TAttribute attribute) where TAttribute : class, ICardAttribute
    {
        var result = _attributeMap.TryGetValue(typeof(TAttribute), out var data);
        attribute = ApplyModifiers(data as TAttribute);
        return result;
    }

    public TAttribute GetAttribute<TAttribute>() where TAttribute : class, ICardAttribute
        => HasAttribute<TAttribute>(out var attr) ? attr : null;

    public void AddModifier<TAttribute>(Func<TAttribute, TAttribute> modifier)
        where TAttribute : class, ICardAttribute
    {
        var type = typeof(TAttribute);

        if (!_modifiers.ContainsKey(type))
            _modifiers[type] = [];

        _modifiers[type].Add(modifier);
    }

    public void ClearModifiers<TAttribute>() where TAttribute : class, ICardAttribute
    {
        if (_modifiers.TryGetValue(typeof(TAttribute), out var modifierList))
            modifierList.Clear();
    }

    private TAttribute ApplyModifiers<TAttribute>(TAttribute original) where TAttribute : class, ICardAttribute
    {
        if (original == null)
            return null;

        var type = typeof(TAttribute);

        if (!_modifiers.TryGetValue(type, out var modifierList))
            return original;

        return modifierList.Cast<Func<TAttribute, TAttribute>>()
            .Aggregate(original, (current, modifier) => modifier(current));
    }
}
