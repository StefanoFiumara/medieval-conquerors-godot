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
    
    // TODO: Implement randomness without using Godot randomizer
    // public static void Shuffle<T>(this List<T> list)
    // {
    //     // NOTE: Fisher Yates shuffle -> https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    //     int n = list.Count;
    //     for (int i = 0; i < n - 1; i++)
    //     {
    //         int r = Rng.RandiRange(i, n);
    //             
    //         (list[r], list[i]) = (list[i], list[r]);
    //     }
    // }
        
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