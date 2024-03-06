namespace MedievalConquerors.Extensions;

public static class TextExtensions
{
    public static string Center(this string str) => $"[center]{str}[/center]";
    public static string Right(this string str) => $"[right]{str}[/right]";
    public static string Italics(this string str) => $"[i]{str}[/i]";
}