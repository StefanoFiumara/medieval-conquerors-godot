using System.Collections.Generic;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Attributes;

public record TargetSelectorAttribute : CardAttribute
{
    public TargetSelector Selector { get; init; }

    public List<Vector2I> SelectTargets(IGame game, Card card) => Selector.SelectTargets(game, card);
}

public abstract record TargetSelector
{
    public abstract List<Vector2I> SelectTargets(IGame game, Card card);
}
