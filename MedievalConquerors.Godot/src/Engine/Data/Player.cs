using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Data.Attributes;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.Data;

public class Player
{
    public int Id { get; }
    
    public TileData TownCenter { get; set; }
    public int InfluenceRange { get; set; }
    
    public ResourceBank Resources { get; }
    
    public AgeType Age { get; private set; }
    
    public List<Card> Deck    { get; } = new();
    public List<Card> Hand    { get; } = new();
    public List<Card> Discard { get; } = new();
    public List<Card> Map     { get; } = new();

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
        };
        
        // TODO: parameterize starting storage limit via game settings
        Resources = new ResourceBank(storageLimit: 30)
        {
            // TODO: Parameterize Starting Resources via game settings
            [ResourceType.Food]  = 10,
            [ResourceType.Wood]  = 10,
            [ResourceType.Gold]  = 5,
            [ResourceType.Stone] = 0
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

