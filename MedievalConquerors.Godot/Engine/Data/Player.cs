using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data;

public interface IPlayer
{
    List<Card> Deck { get; }
    List<Card> Hand { get; }
    List<Card> Discard { get; }
    List<Card> Board { get; }
}

public class Player : IPlayer
{
    public List<Card> Deck { get; } = new();
    public List<Card> Hand { get; } = new();
    public List<Card> Discard { get; } = new();
    public List<Card> Board { get; } = new();
}

