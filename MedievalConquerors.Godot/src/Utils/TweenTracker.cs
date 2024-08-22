using System.Collections.Generic;
using Godot;

namespace MedievalConquerors.Utils;

/// <summary>
/// Helper class to track active tweens on specific nodes.
/// Kills existing tweens when a new tween on a node is added.
/// </summary>
/// <typeparam name="T">The type of object to track tweens for.</typeparam>
public class TweenTracker<T> where T : GodotObject
{
    private readonly Dictionary<T, Tween> _activeTweens = new();

    public bool IsTweening(T node) => _activeTweens.ContainsKey(node);

    public void TrackTween(Tween tween, T target)
    {
        if (IsTweening(target))
        {
            _activeTweens[target].Kill();
            _activeTweens.Remove(target);
        }
		
        tween.Chain().TweenCallback(Callable.From(() => _activeTweens.Remove(target)));
        _activeTweens.Add(target, tween);
    }
}