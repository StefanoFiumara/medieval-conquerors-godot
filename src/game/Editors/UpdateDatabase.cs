using Godot;
using System;
using System.IO;
using LiteDB;
using MedievalConquerors.Engine.Data;

public partial class UpdateDatabase : Button
{
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		// var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "GameData", "CardLibrary.db");
		// using var database = new LiteDatabase($"Filename={dbPath}");
		// BsonMapper.Global.EnumAsInteger = true;
		//
		// var collection = database.GetCollection("CardData");
		// foreach (var doc in collection.FindAll())
		// {
		// 	doc["CardPortraitUid"] = doc["ImagePath"]; // copy value
		// 	doc.Remove("ImagePath");            // remove old key
		// 	doc["TokenFrameId"] = 0;
		// 	collection.Update(doc);
		// }
	}
}
