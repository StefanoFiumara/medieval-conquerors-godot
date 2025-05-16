using Godot;

namespace MedievalConquerors.Engine.Extensions;

public static class NodeExtensions
{
    public static T SearchParent<T>(this Node node) where T : Node
    {
        var parent = node.GetParentOrNull<Node>();
        while (parent != null)
        {
            if (parent is T match)
                return match;

            parent = parent.GetParentOrNull<Node>();
        }

        return null;
    }
}
