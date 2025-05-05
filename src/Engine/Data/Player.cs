using System.Collections.Generic;

namespace MedievalConquerors.Engine.Data;

public class Player
{
    public int Id { get; }

    public TileData TownCenter { get; set; }
    public int InfluenceRange { get; set; }

    public AgeType Age { get; private set; }
    public ResourceBank Resources { get; } = new();

    public List<Card> Deck     { get; } = [];
    public List<Card> Hand     { get; } = [];
    public List<Card> Map      { get; } = [];
    public List<Card> Discard  { get; } = [];
    public List<Card> Banished { get; } = [];

    public int TurnStartDrawCount => 3 + (int)Age;

    private readonly Dictionary<Zone, List<Card>> _zoneMap;

    public Player(int id)
    {
        Id = id;
        Age = AgeType.DarkAge;
        InfluenceRange = 3;

        _zoneMap = new Dictionary<Zone, List<Card>>
        {
            { Zone.Deck, Deck },
            { Zone.Hand, Hand },
            { Zone.Discard, Discard },
            { Zone.Map, Map },
            { Zone.Banished, Banished }
        };
    }

    public List<Card> this[Zone z] => _zoneMap.GetValueOrDefault(z);

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
            MoveCard(card, toZone);
    }
}
