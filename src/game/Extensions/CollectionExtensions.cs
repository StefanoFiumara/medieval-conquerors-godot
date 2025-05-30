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
        // TODO: Make rng globally available so that we can control the seed.
        // IDEA: Add a `RandomnessSystem` to control current seed and act as proxy for an RNG instance
        var rng = new Random();

        // TODO: There exists an rng.Shuffle method that does an in-place shuffle, is that a good candidate to replace this?
        for (var i = list.Count - 1; i > 0; --i)
        {
            // Select a random index
            var r = rng.Next(0, i + 1);
            // Swap
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    public static T GetRandom<T>(this List<T> list)
    {
        // TODO: Make rng globally available so that we can control the seed.
        var rng = new Random();
        var randomIndex = rng.Next(0, list.Count);
        return list[randomIndex];
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
