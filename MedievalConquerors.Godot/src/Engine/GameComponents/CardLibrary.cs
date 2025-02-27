using System.Collections.Generic;
using System.Collections.Immutable;
using MedievalConquerors.Editor.DataAccess;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine.GameComponents;

public class CardLibrary : GameComponent, IAwake
{
	public const int TownCenterId = 12;

	private ILogger _logger;
	private IReadOnlyDictionary<int, CardData> _cardData;

	public CardLibrary() { }
	public CardLibrary(List<CardData> cardData)
	{
		_cardData = cardData.ToImmutableDictionary(c => c.Id);
	}
	
	public void Awake()
	{
		_logger = Game.GetComponent<ILogger>();
		if (_cardData is null || _cardData.Count == 0)
		{
			using var db = new CardDatabase();
			_cardData = db.Query.ToList().ToImmutableDictionary(c => c.Id);
		}
	}

	public Card LoadCard(int id, Player owner)
	{
		if (_cardData.TryGetValue(id, out var data))
		{
			// TODO: Do we need to clone the card data here in case the data gets mutated during gameplay?
			// It should not be a problem because the card attributes already get cloned, and the other values should not change.
			return new Card(data, owner);
		}
		
		_logger.Warn($"Could not find card with ID {id}");
		return null;
	}
	
	// TODO: Formalize deckInfo tuple into separate object
	public List<Card> LoadDeck(Player owner, List<(int cardId, int amount)> deckInfo)
	{
		var result = new List<Card>();
		foreach (var (id, amount) in deckInfo)
		{
			for (int i = 0; i < amount; i++)
			{
				var card = LoadCard(id, owner);
				if(card != null)
					result.Add(card);
			}
		}

		return result;
	}
}
