using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record ActionDefinition
{
    public string ActionType { get; init; }
    public string Data { get; init; }

    private Lazy<ImmutableDictionary<string, string>> LazyData => new(ParseData);

    private ImmutableDictionary<string, string> ParsedData => LazyData.Value;

    public T GetData<T>(string key)
    {
        if (!ParsedData.TryGetValue(key, out var value))
            return default;

        if (typeof(T).IsEnum)
            return (T) Enum.Parse(typeof(T), value);

        return (T) Convert.ChangeType(value, typeof(T));
        // TODO: Should we throw an error here instead of returning a default value?
    }

    private ImmutableDictionary<string, string> ParseData()
    {
        return Data.Split(',')
            .Select(pair => pair.Split('='))
            .ToImmutableDictionary(parts => parts[0], parts => parts[1]);
    }
}

public abstract record AbilityAttribute : ICardAttribute
{
    public IReadOnlyList<ActionDefinition> Actions { get; init; } = [];
}

public record OnCardPlayedAbility : AbilityAttribute;
public record OnCardActivatedAbility : AbilityAttribute;
