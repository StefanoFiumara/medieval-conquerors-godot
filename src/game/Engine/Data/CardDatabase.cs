using System;
using System.IO;
using LiteDB;

namespace MedievalConquerors.Engine.Data;

public sealed class CardDatabase : IDisposable
{
	private readonly ILiteCollection<CardData> _cardCollection;
	private readonly LiteDatabase _database;

	public ILiteQueryable<CardData> Query => _cardCollection.Query();

	public CardDatabase()
	{
		var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "GameData", "CardLibrary.db");
		_database = new LiteDatabase($"Filename={dbPath};Connection=shared");
		BsonMapper.Global.EnumAsInteger = true;

		_cardCollection = _database.GetCollection<CardData>();
		_cardCollection.EnsureIndex(x => x.Id, true);
	}

	public int SaveCardData(CardData data)
	{
		if (data.Id == 0)
			return _cardCollection.Insert(data);

		_cardCollection.Update(data.Id, data);
		return data.Id;
	}

	public bool DeleteCardData(CardData data) => DeleteCardData(data.Id);
	public bool DeleteCardData(int dataId)
	{
		return _cardCollection.Delete(dataId);
	}

	public void Dispose()
	{
		_database?.Dispose();
	}
}
