using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Engine.Data;

public class Player
{
    public int Id { get; }
    
    public TileData TownCenter { get; set; }
    public int InfluenceRange { get; set; }
    
    public ResourceBank Resources { get; }
    
    public List<Card> Deck    { get; } = new();
    public List<Card> Hand    { get; } = new();
    public List<Card> Discard { get; } = new();
    public List<Card> Map     { get; } = new();

    private readonly Dictionary<Zone, List<Card>> _zoneMap;

    public Player(int id)
    {
        Id = id;
        _zoneMap = new Dictionary<Zone, List<Card>>
        {
            { Zone.Deck, Deck },
            { Zone.Hand, Hand },
            { Zone.Discard, Discard },
            { Zone.Map, Map },
        };
        
        InfluenceRange = 3;
        
        // TODO: parameterize starting storage limit
        Resources = new ResourceBank(storageLimit: 60)
        {
            // TODO: Parameterize Starting Resources
            [ResourceType.Food] = 40,
            [ResourceType.Gold] = 20
        };

        // TEMP: Add some temporary cards
        Deck.AddRange(Enumerable.Range(0, 30)
            .Select(i => new Card(
                new CardData
                {
                    Title = $"Knight {i}",
                    Description = $"Mighty Mounted Royal Warrior {i}",
                    ImagePath = "res://Assets/CardImages/knight.png",
                    CardType = CardType.Unit,
                    Tags = Tags.None,
                    Attributes = new()
                    {
                        new ResourceCostAttribute { Food = 4, Gold = 2 },
                        new MovementAttribute { Distance = 2 },
                        // TODO: Test with other buildings once available
                        new SpawnPointAttribute { SpawnTags = Tags.TownCenter }
                    }
                }, this)));
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

