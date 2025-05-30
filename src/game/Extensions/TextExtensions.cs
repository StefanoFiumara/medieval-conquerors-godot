using System.Text;

namespace MedievalConquerors.Extensions;

public static class TextExtensions
{
    public static string Center(this string str) => $"[center]{str}[/center]";
    public static string Italics(this string str) => $"[i]{str}[/i]";

    public static string PrettyPrint(this string str)
    {
        if (string.IsNullOrEmpty(str)) 
        {
            return string.Empty;
        }

        var result = new StringBuilder(str.Length * 2);
        result.Append(str[0]);
    
        for (int i = 1; i < str.Length; i++) 
        {
            if (char.IsUpper(str[i])) 
                result.Append(' ');

            result.Append(str[i]);
        }

        return result.ToString();
    }
}