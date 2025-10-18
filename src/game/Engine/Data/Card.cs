using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using LiteDB;
using Riok.Mapperly.Abstractions;

namespace MedievalConquerors.Engine.Data;

public class Card
{
	private readonly IReadOnlyDictionary<Type, ICardAttribute> _attributeMap;

	public CardData Data { get; }

	public Player Owner { get; }
	public Zone Zone { get; set; }
	public Vector2I MapPosition { get; set; }

	public Card(CardData data, Player owner, Zone zone = Zone.None, Vector2I mapPosition = default)
	{
		Data = data;
		Owner = owner;
		Zone = zone;
		MapPosition = Zone == Zone.Map ? mapPosition : HexMap.None;

		// TODO: Add a system for attribute modifications, rather than relying on mutable card attributes.
		// This way we can get rid of the clunky Clone() methods
		_attributeMap = Data.Attributes.Select(attr =>
			{
				var copy = attr.Clone();
				copy.Owner = this;
				return copy;
			})
			.ToImmutableDictionary(attr => attr.GetType(), attr => attr);
	}

	public bool HasAttribute<TAttribute>(out TAttribute attribute) where TAttribute : class, ICardAttribute
	{
		var result = _attributeMap.TryGetValue(typeof(TAttribute), out var data);
		attribute = data as TAttribute;
		return result;
	}

	public TAttribute GetAttribute<TAttribute>() where TAttribute : class, ICardAttribute
		=> HasAttribute<TAttribute>(out var attr) ? attr : null;
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
	public IReadOnlyList<ICardAttribute> Attributes { get; init; } = [];
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
