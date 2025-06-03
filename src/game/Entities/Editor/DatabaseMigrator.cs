using Godot;

namespace MedievalConquerors.Entities.Editor;

public partial class DatabaseMigrator : Button
{
	public override void _EnterTree()
	{
		Pressed += OnPressed;
	}
	
	public override void _ExitTree() 
	{
		Pressed -= OnPressed;
	}

	private void OnPressed()
	{
		// using var cardDb = new CardDatabase();
		// var cards = cardDb.Query.ToList();
		//
		// cardDb.Database.GetCollection<CardData>().Update(cards);
	}
}
