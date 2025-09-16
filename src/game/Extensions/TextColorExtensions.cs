using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Extensions;

public static class TextColorExtensions
{
    public static string Black(this string msg)   => WrapInColor(msg, "black");
    public static string Red(this string msg)     => WrapInColor(msg, "red");
    public static string Green(this string msg)   => WrapInColor(msg, "green");
    public static string Yellow(this string msg)  => WrapInColor(msg, "yellow");
    public static string Blue(this string msg)    => WrapInColor(msg, "blue");
    public static string Magenta(this string msg) => WrapInColor(msg, "magenta");
    public static string Pink(this string msg)    => WrapInColor(msg, "pink");
    public static string Purple(this string msg)  => WrapInColor(msg, "purple");
    public static string Cyan(this string msg)    => WrapInColor(msg, "cyan");
    public static string White(this string msg)   => WrapInColor(msg, "white");
    public static string Orange(this string msg)  => WrapInColor(msg, "orange");
    public static string Gray(this string msg)    => WrapInColor(msg, "gray");

    private static string WrapInColor(string msg, string color) => $"[color={color}]{msg}[/color]";
}

public static class ResourceColors
{
    public static readonly Dictionary<ResourceType, string> Map = new()
    {
        { ResourceType.Food,  FOOD },
        { ResourceType.Wood,  WOOD },
        { ResourceType.Gold,  GOLD },
        { ResourceType.Stone, STONE },
    };

    public const string FOOD = "#f76a67";
    public const string WOOD = "#ab7b64";
    public const string GOLD = "#fcdd00";
    public const string STONE = "#c7d1db";
}
