using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record ActionDefinition
{
    public string ActionType { get; init; }
    // TODO: Can we actually just serialize the dictionary into the DB instead of doing this parsing?
    //       Check LiteDB's ability to serialize Dictionaries
    public string Data { get; init; }

    private Lazy<ImmutableDictionary<string, string>> LazyData => new(ParseData);
    private ImmutableDictionary<string, string> ParsedData => LazyData.Value;

    public T GetData<T>(string key) => (T) GetData(key, typeof(T));
    public object GetData(string key, Type type)
    {
        var result = ParsedData.GetValueOrDefault(key);

        return type.IsEnum
            ? Enum.Parse(type, result)
            : Convert.ChangeType(result, type);
    }

    private ImmutableDictionary<string, string> ParseData()
    {
        return Data.Split(',')
            .Select(pair => pair.Split('='))
            .ToImmutableDictionary(parts => parts[0], parts => parts[1]);
    }
}

public abstract record AbilityAttribute : CardAttribute
{
    public IReadOnlyList<ActionDefinition> Actions { get; init; } = [];
}

public record OnCardPlayedAbility : AbilityAttribute;
public record OnCardActivatedAbility : AbilityAttribute;
