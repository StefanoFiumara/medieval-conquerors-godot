using Godot;

namespace MedievalConquerors.Extensions;

public static class VectorExtensions
{
    public static bool IsValid(this Vector2I v) => v.X != int.MinValue && v.Y != int.MinValue;
}