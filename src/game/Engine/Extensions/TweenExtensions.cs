using Godot;

namespace MedievalConquerors.Engine.Extensions;

public static class TweenExtensions
{
    extension(Node2D parent)
    {
        public Tween NullTween
        {
            get
            {
                var nullTween = parent.CreateTween();
                nullTween.TweenInterval(0);
                return nullTween;
            }
        }
    }
}
