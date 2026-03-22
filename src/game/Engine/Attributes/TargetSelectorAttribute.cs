using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record TargetSelectorAttribute : CardAttribute
{
    public TargetSelector Selector { get; init; }
}

public abstract record TargetSelector
{
    protected abstract List<Vector2I> SelectTargets(IGame game, Card card);
}
