using System;
using System.IO;
using LiteDB;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Addons.CardDataEditor;

public class CardDatabase : IDisposable
{
    private readonly LiteDatabase _cardDatabase;
    private readonly ILiteCollection<CardData> _cardCollection;

    public ILiteQueryable<CardData> Query => _cardCollection.Query();
    public CardDatabase()
    {
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "GameData", "CardLibrary.db");
        _cardDatabase = new LiteDatabase(dbPath);
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

    public CardData LoadCardData(int cardId)
    {
        return _cardCollection.FindById(cardId);
    }

    public void Dispose()
    {
        _cardDatabase?.Dispose();
    }
}