using System.IO;
using System.Linq;
using Godot;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.entities.editor;

public partial class DatabaseMigrator : Button
{
	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		// using var cardDb = new CardDatabase();
		// var cards = cardDb.Query.ToList();
		//
		// cardDb.Database.GetCollection<CardData>().Update(cards);
	}
}
