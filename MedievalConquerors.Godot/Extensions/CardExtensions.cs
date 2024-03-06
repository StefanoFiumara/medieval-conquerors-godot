using System.Linq;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Extensions;

public static class CardExtensions
{
    public static TAttribute GetDataAttribute<TAttribute>(this CardData data)
    {
        var attribute = data.Attributes.OfType<TAttribute>().SingleOrDefault();
        return attribute;
    }

    public static TAttribute GetAttribute<TAttribute>(this Card card)
        where TAttribute : ICardAttribute
    {
        var attribute = card.Attributes.OfType<TAttribute>().SingleOrDefault();
        return attribute;
    }
}