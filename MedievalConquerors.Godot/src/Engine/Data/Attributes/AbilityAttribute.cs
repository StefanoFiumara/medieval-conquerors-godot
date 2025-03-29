using System;
using System.Collections.Generic;
using System.Linq;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Data.Attributes;

public class ActionDefinition
{
    public string ActionType { get; set; }
    public string Data { get; set; }

    [MapperIgnore]
    private Dictionary<string, string> ParsedData => LazyData.Value;

    public T GetData<T>(string key)
    {
        if (ParsedData.TryGetValue(key, out var value))
        {
            if (typeof(T).IsEnum)
                return (T) Enum.Parse(typeof(T), value);

            return (T) Convert.ChangeType(value, typeof(T));
        }

        // TODO: Should we throw an error here instead of returning a default value?
        return default;
    }
    private Lazy<Dictionary<string, string>> LazyData => new(ParseData);
    private Dictionary<string, string> ParseData()
    {
        return Data.Split(',')
            .Select(pair => pair.Split('='))
            .ToDictionary(parts => parts[0], parts => parts[1]);
    }
}

public abstract class AbilityAttribute : CardAttribute
{
    public List<ActionDefinition> Actions { get; set; }
    // TODO: Target selector?
}

public class OnCardPlayedAbility : AbilityAttribute
{
    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}

public class OnCardActivatedAbility : AbilityAttribute
{
    public override ICardAttribute Clone() => AttributeMapper.Clone(this);
}
