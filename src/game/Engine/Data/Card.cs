using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace MedievalConquerors.Engine.Data;

public class Card
{
	private readonly ImmutableDictionary<Type, ICardAttribute> _attributeMap;

	public CardData Data { get; }

	public Player Owner { get; }
	public Zone Zone { get; set; }
	public Vector2I MapPosition { get; set; }
	// TODO: Implement attribute modifiers here

	public Card(CardData data, Player owner, Zone zone = Zone.None, Vector2I mapPosition = default)
	{
		Data = data;
		Owner = owner;
		Zone = zone;
		MapPosition = Zone == Zone.Map ? mapPosition : HexMap.None;

		_attributeMap = Data.Attributes.ToImmutableDictionary(attr => attr.GetType(), attr => attr);
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

public interface ICardAttribute;
