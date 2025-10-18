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
		// CardData is now immutable, but attributes still need to be cloned because they are modified during gameplay.
		// TODO: Add a buff/debuff system, rather than relying on mutable card attributes.
		foreach (var dataAttribute in Data.Attributes)
		{
			var attributeCopy = dataAttribute.Clone();
			attributeCopy.Owner = this;
			Attributes.Add(dataAttribute.GetType(), attributeCopy);
		}
	}
}

public record CardData
{
	public int Id { get; init; }
	public string Title { get; init; }
	public string Description { get; init; }
	public string ImagePath { get; init; }
	public string TokenImagePath { get; init; }
	public CardType CardType { get; init; }
	public Tags Tags { get; init; }
	public List<ICardAttribute> Attributes { get; init; } = new();
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
