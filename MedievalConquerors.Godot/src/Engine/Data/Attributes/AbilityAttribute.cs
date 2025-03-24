using System;
using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data.Attributes;

public class ActionDefinition
{
    // TODO: better types for this, json?
    public string ActionType { get; set; }
    public string Parameters { get; set; }
}

public class AbilityAttribute : ICardAttribute
{
    public List<ActionDefinition> Actions { get; set; }
    public AbilityTrigger Trigger { get; set; }

    public ICardAttribute Clone() => AttributeMapper.Clone(this);
}
