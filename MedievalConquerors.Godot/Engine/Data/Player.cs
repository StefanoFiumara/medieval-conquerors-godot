using System.Collections.Generic;
using System.Linq;
using MedievalConquerors.Engine.Data.Attributes;

namespace MedievalConquerors.Engine.Data;

public interface IPlayer
{
    int Id { get; }
    ITileData TownCenter { get; set; }
    ResourceBank Resources { get; }
    List<Card> Deck { get; }
    List<Card> Hand { get; }
    List<Card> Discard { get; }
    List<Card> Map { get; }

    List<Card> this[Zone z] { get; }
        
    void MoveCard(Card target, Zone toZone);
    void MoveCards(List<Card> targets, Zone toZone);
}

public class Player : IPlayer
{
    public int Id { get; }
    public ITileData TownCenter { get; set; }
    public ResourceBank Resources { get; }
    public List<Card> Deck { get; } = new();
    public List<Card> Hand { get; } = new();
    public List<Card> Discard { get; } = new();
    public List<Card> Map { get; } = new();

    private readonly Dictionary<Zone, List<Card>> _zoneMap;

    public Player(int id)
    {
        Id = id;
        Resources = new();
        
        _zoneMap = new Dictionary<Zone, List<Card>>
        {
            { Zone.Deck, Deck },
            { Zone.Hand, Hand },
            { Zone.Discard, Discard },
            { Zone.Map, Map },
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
                        new MovementAttribute { Distance = 10 }
                    }
                }, this)));

        // TEMP: Add some starting resources
        Resources.Food = 40;
        Resources.Gold = 20;
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

