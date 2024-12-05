using System.Collections.Generic;
using MedievalConquerors.Editor.DataAccess;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class CardRepository : GameComponent, IAwake
{
    private ILogger _logger;
    
    public void Awake()
    {
        // TODO: Do we want to load the card library into an in-memory collection here?
        _logger = Game.GetComponent<ILogger>();
    }

    // TODO: Formalize deckInfo tuple into separate object
    public List<Card> LoadPlayerDeck(Player owner, List<(int cardId, int amount)> deckInfo)
    {
        // TODO: Validate that this works with the exported game and that it can read the .db file
        // If it doesn't work, we may need to adjust CardDatabase to create a copy of the db file inside the res:// folder to somewhere else on the file system
        using var db = new CardDatabase();

        var result = new List<Card>();

        foreach (var (id, amount) in deckInfo)
        {
            for (int i = 0; i < amount; i++)
            {
                // TODO: More efficient way to query for all required IDs.
                // IDEA: Load all card data from disk and hold it in memory, then grab copies from that cache to build individual cards.
                var data = db.Query.Where(c => c.Id == id).SingleOrDefault();
                if (data == null)
                {
                    _logger.Warn($"Could not find card with ID {id} while loading player decks.");
                    break;
                }
                    
                result.Add(new Card(data, owner));
            }
        }

        return result;
    }
}