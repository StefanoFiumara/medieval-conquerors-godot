using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data.Attributes;

public class ActionDefinition
{
    public string ActionType { get; set; }
    public string Data { get; set; }
}

public class AbilityAttribute : ICardAttribute
{
    public List<ActionDefinition> Actions { get; set; }
    // TODO: Target selector?
    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}

public class OnCardPlayedAbility : AbilityAttribute { }
public class OnCardActivatedAbility : AbilityAttribute { }
