using LiteDB;
using MedievalConquerors.Engine.Data;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Attributes;

public record MovementAttribute : ICardAttribute
{
    public int Distance { get; init; }

    // TODO: Reimplement this logic with the modifier system
    // public bool CanMove(int amount) => RemainingDistance >= amount;
    // public void Move(int amount) => RemainingDistance -= amount;
}
