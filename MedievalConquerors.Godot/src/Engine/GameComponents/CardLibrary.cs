using System.Collections.Generic;
using MedievalConquerors.Editor.DataAccess;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class CardLibrary : GameComponent, IAwake
{
    public const int TownCenterId = 12;

    private ILogger _logger;
    
    public void Awake()
    {
        _logger = Game.GetComponent<ILogger>();
    }

    public Card LoadCard(int id, Player owner)
    {
        using var db = new CardDatabase();
        return LoadCard(db, id, owner);
    }
    
    private Card LoadCard(CardDatabase db, int id, Player owner)
    {
        var data = db.Query.Where(c => c.Id == id).SingleOrDefault();
        if (data == null)
        {
            _logger.Warn($"Could not find card with ID {id}");
            return null;
        }

        return new Card(data, owner);
    }
    // TODO: Formalize deckInfo tuple into separate object
    public List<Card> LoadDeck(Player owner, List<(int cardId, int amount)> deckInfo)
    {
        var result = new List<Card>();
        
        using var db = new CardDatabase();
        foreach (var (id, amount) in deckInfo)
        {
            for (int i = 0; i < amount; i++)
            {
                // TODO: optimize query so we can get all info in one trip to the disk.
                // IDEA: Load all card data from disk and hold it in memory, then grab copies from that cache to build individual cards.
                var card = LoadCard(db, id, owner);
                if(card != null)
                    result.Add(card);
            }
        }

        return result;
    }
}