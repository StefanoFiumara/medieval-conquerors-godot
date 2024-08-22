using System.Collections.Generic;

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