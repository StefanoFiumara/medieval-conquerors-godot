using System.Collections.Generic;
using MedievalConquerors.Engine.Extensions;

namespace MedievalConquerors.Engine.Data;

public class Player
{
    public int Id { get; }

    public TileData TownCenter { get; set; }
    public int InfluenceRange { get; set; }

    public AgeType Age { get; private set; }
    public ResourceBank Resources { get; } = new();

    private readonly List<Card> _deck     = [];
    private readonly List<Card> _hand     = [];
    private readonly List<Card> _map      = [];
    private readonly List<Card> _discard  = [];
    private readonly List<Card> _banished = [];

    public IReadOnlyList<Card> Deck     => _deck.AsReadOnly();
    public IReadOnlyList<Card> Hand     => _hand.AsReadOnly();
    public IReadOnlyList<Card> Map      => _map.AsReadOnly();
    public IReadOnlyList<Card> Discard  => _discard.AsReadOnly();
    public IReadOnlyList<Card> Banished => _banished.AsReadOnly();


    public int TurnStartDrawCount => 4 + (int)Age;

    private readonly Dictionary<Zone, List<Card>> _zoneMap;

    public Player(int id)
    {
        Id = id;
        Age = AgeType.DarkAge;
        InfluenceRange = 3;

        _zoneMap = new Dictionary<Zone, List<Card>>
        {
            { Zone.Deck, _deck },
            { Zone.Hand, _hand },
            { Zone.Discard, _discard },
            { Zone.Map, _map },
            { Zone.Banished, _banished }
        };
    }

    public IReadOnlyList<Card> this[Zone z] => _zoneMap.GetValueOrDefault(z).AsReadOnly();

    public void MoveCard(Card target, Zone toZone)
    {
        var fromZone = _zoneMap.GetValueOrDefault(target.Zone);
        var targetZone = _zoneMap.GetValueOrDefault(toZone);

        fromZone?.Remove(target);
        targetZone?.Add(target);

        target.Zone = toZone;
    }

    public void ShuffleDeck() => _deck.Shuffle();

    public void MoveCards(List<Card> targets, Zone toZone)
    {
        foreach (var card in targets)
            MoveCard(card, toZone);
    }
}
