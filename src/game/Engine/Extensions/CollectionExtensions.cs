using System;
using System.Collections.Generic;
using System.Linq;

namespace MedievalConquerors.Engine.Extensions;

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

    // IDEA: Add a `RandomnessSystem` to control current seed and act as proxy for an RNG instance
    // TODO: Upgrade to PCG32 randomness instead of relying on System.Random
    //          - May need to import 3rd party library
    //          - Could Godot's RandomNumberGenerator, which uses PCG32, but it does not play nice with unit tests that run outside of the engine.

    public static void Shuffle<T>(this List<T> list)
    {
        // NOTE: Fisher Yates shuffle -> https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        var rng = new Random();

        for (var i = list.Count - 1; i > 0; --i)
        {
            // Select a random index
            var r = rng.Next(0, i);
            // Swap
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    public static T GetRandom<T>(this List<T> list)
    {
        var rng = new Random();
        var randomIndex = rng.Next(0, list.Count - 1);
        return list[randomIndex];
    }

    public static IEnumerable<T> Draw<T>(this List<T> list, int amount)
        where T : class
    {
        int resultCount = Math.Min(amount, list.Count);

        for (int i = 0; i < resultCount; i++)
        {
            var item = list.TakeTop();
            yield return item;
        }
    }
}
