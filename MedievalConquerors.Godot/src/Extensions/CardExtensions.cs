using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Extensions;

public static class CardExtensions
{
    public static TAttribute GetAttribute<TAttribute>(this Card card)
        where TAttribute : class, ICardAttribute
    {
        if(card.Attributes.TryGetValue(typeof(TAttribute), out var attribute))
        {
            return attribute as TAttribute;
        }

        return null;
    }
}