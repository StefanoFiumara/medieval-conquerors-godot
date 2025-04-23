using System;
using System.IO;
using LiteDB;

namespace MedievalConquerors.Engine.Data;

public sealed class CardDatabase : IDisposable
{
	private readonly LiteDatabase _cardDatabase;
	private readonly ILiteCollection<CardData> _cardCollection;

	public ILiteQueryable<CardData> Query => _cardCollection.Query();
	
	public CardDatabase()
	{
		var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "game_data", "CardLibrary.db");
		_cardDatabase = new LiteDatabase(dbPath);
		BsonMapper.Global.EnumAsInteger = true;
		
		_cardCollection = _cardDatabase.GetCollection<CardData>();
		_cardCollection.EnsureIndex(x => x.Id, true);
	}

	public int SaveCardData(CardData data)
	{
		if (data.Id == 0) 
			return _cardCollection.Insert(data);
		
		_cardCollection.Update(data.Id, data);
		return data.Id;
	}

	public bool DeleteCardData(CardData data)
	{
		return _cardCollection.Delete(data.Id);
	}
	
	public void Dispose()
	{
		_cardDatabase?.Dispose();
	}
}
