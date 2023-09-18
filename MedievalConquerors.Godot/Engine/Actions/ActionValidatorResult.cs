using System.Collections.Generic;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Events;

namespace MedievalConquerors.Engine.Actions;

public class ActionValidatorResult
{
    public bool IsValid => ValidationErrors.Count == 0;

    public List<string> ValidationErrors { get; } = new();

    public void Invalidate(string reason)
    {
        ValidationErrors.Add(reason);
    }
}