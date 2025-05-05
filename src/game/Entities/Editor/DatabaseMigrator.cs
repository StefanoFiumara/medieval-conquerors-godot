using Godot;

namespace MedievalConquerors.Entities.Editor;

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
