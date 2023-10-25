using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data;

public interface IPlayer
{
    List<Card> Deck { get; }
    List<Card> Hand { get; }
    List<Card> Discard { get; }
    List<Card> Board { get; }

    List<Card> this[Zone z] { get; }
        
    void MoveCard(Card target, Zone toZone);
    void MoveCards(List<Card> targets, Zone toZone);
}

public class Player : IPlayer
{
    public List<Card> Deck { get; } = new();
    public List<Card> Hand { get; } = new();
    public List<Card> Discard { get; } = new();
    public List<Card> Board { get; } = new();

    private readonly Dictionary<Zone, List<Card>> _zoneMap;

    public Player()
    {
        _zoneMap = new Dictionary<Zone, List<Card>>
        {
            { Zone.Deck, Deck },
            { Zone.Hand, Hand },
            { Zone.Discard, Discard },
            { Zone.Board, Board },
        };
    }
    public List<Card> this[Zone z] => _zoneMap.ContainsKey(z) ? _zoneMap[z] : null;

    public void MoveCard(Card target, Zone toZone)
    {
        var fromZone = this[target.Zone];
        var targetZone = this[toZone];
        
        fromZone?.Remove(target);
        targetZone?.Add(target);

        target.Zone = toZone;
    }

    public void MoveCards(List<Card> targets, Zone toZone)
    {
        foreach (var card in targets)
        {
            MoveCard(card, toZone);
        }
    }
}

