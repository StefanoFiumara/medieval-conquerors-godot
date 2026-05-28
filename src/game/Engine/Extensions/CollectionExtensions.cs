using System;
using System.Collections.Generic;

namespace MedievalConquerors.Engine.Extensions;

// TODO: Covert this to a game system so things like the randomness seed and RNG instance can be shared across game systems
public static class CollectionExtensions
{
    public static IEnumerable<T> Draw<T>(this IReadOnlyList<T> list, int amount)
    {
        amount  =  Math.Min(amount, list.Count);
        for (int i = 0; i < amount; i++)
            yield return list[^(i+1)];
    }

    public static void Shuffle<T>(this List<T> list)
    {
        // IDEA: Add a `RandomnessSystem` to control current seed and act as proxy for an RNG instance
        // TODO: Upgrade to PCG32 randomness instead of relying on System.Random
        //          - May need to import 3rd party library
        //          - Could use Godot's RandomNumberGenerator, which uses PCG32, but it does not play nice with unit tests that run outside of the engine.

        // NOTE: Fisher Yates shuffle -> https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        // TODO: Look into StS stable shuffle for multiplayer consistency (sort first, then shuffle?)
        var rng = new Random();

        for (var i = list.Count - 1; i > 0; --i)
        {
            // Select a random index
            var r = rng.Next(0, i);
            // Swap
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}
