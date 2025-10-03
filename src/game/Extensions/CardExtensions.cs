using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Extensions;

public static class CardExtensions
{
    public static bool HasAttribute<TAttribute>(this Card card, out TAttribute attribute)
        where TAttribute : class, ICardAttribute
    {
        var result = card.Attributes.TryGetValue(typeof(TAttribute), out var data);
        attribute = data as TAttribute;
        return result;
    }

    public static TAttribute GetAttribute<TAttribute>(this Card card) where TAttribute : class, ICardAttribute
        => card.HasAttribute<TAttribute>(out var attr) ? attr : null;
}
