using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data;

public abstract record CardAttribute;

public record CardData
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string ImagePath { get; init; }
    public string TokenImagePath { get; init; }
    public CardType CardType { get; init; }
    public Tags Tags { get; init; }
    public IReadOnlyList<CardAttribute> Attributes { get; init; } = [];
}
