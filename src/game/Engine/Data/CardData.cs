using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data;

public record CardData
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string ImagePath { get; init; }
    public string TokenImagePath { get; init; }
    public CardType CardType { get; init; }
    public Tags Tags { get; init; }
    public IReadOnlyList<ICardAttribute> Attributes { get; init; } = [];
}

public interface ICardAttribute;
public interface IModifier;

public abstract record Modifier<TAttribute> : IModifier
    where TAttribute : class, ICardAttribute
{
    public abstract TAttribute Apply(TAttribute original);
}
