using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MedievalConquerors.Extensions;

public static class CollectionExtensions
{    
    public static T TakeTop<T>(this List<T> list) where T : class
    {
        if (list.Count == 0)
        {
            return null;
        }
             
        var card = list.Last();
        list.Remove(card);

        return card;
    }
    
    public static void Shuffle<T>(this List<T> list)
    {
        // NOTE: Fisher Yates shuffle -> https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        // TODO: make rng a globally available property so that we can control the seed.
        var rng = new Random();

        for (var i = list.Count - 1; i > 0; --i)
        {
            // Select a random index
            var r = rng.Next(0, i + 1);
            // Swap
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
        
    public static IEnumerable<T> Draw<T>(this List<T> list, int amount) 
        where T : class
    {
        int resultCount = Mathf.Min(amount, list.Count);
            
        for (int i = 0; i < resultCount; i++)
        {
            var item = list.TakeTop();
            yield return item;
        }
    }
}