using System;
using System.Collections.Generic;
using Godot;
using LiteDB;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Data;

public class Card
{
	public CardData Data { get; }
	public Dictionary<Type, ICardAttribute> Attributes { get; }
	public Player Owner { get; }
	public Zone Zone { get; set; }
	public Vector2I MapPosition { get; set; }

	public Card(CardData data, Player owner, Zone zone = Zone.None, Vector2I mapPosition = default)
	{
		Data = data;
		Owner = owner;
		Zone = zone;
		MapPosition = Zone == Zone.Map ? mapPosition : HexMap.None;

		Attributes = new();

		// NOTE: Copy the card attributes from CardData into our state, so we can modify them without affecting the originals
		// TODO: Add a buff/debuff system, rather than relying on mutable card attributes
		//		This way, we can make everything related to CardData Immutable, and the Card object can hold any state necessary without cloning attributes.
		foreach (var dataAttribute in Data.Attributes)
		{
			var attributeCopy = dataAttribute.Clone();
			attributeCopy.Owner = this;
			Attributes.Add(dataAttribute.GetType(), attributeCopy);
		}
	}
}

public class CardData
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string ImagePath { get; set; }
	public string TokenImagePath { get; set; }
	public CardType CardType { get; set; }
	public Tags Tags { get; set; }
	public List<ICardAttribute> Attributes { get; set; } = new();
}

public abstract class CardAttribute : ICardAttribute
{
	[MapperIgnore]
	[BsonIgnore]
	public Card Owner { get; set; }

	public abstract ICardAttribute Clone();
}

public interface ICardAttribute
{
	Card Owner { get; set; }
	ICardAttribute Clone();
}
