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
        { ResourceType.Food,  Food },
        { ResourceType.Wood,  Wood },
        { ResourceType.Gold,  Gold },
        { ResourceType.Stone, Stone },
    };
    
    public const string Food = "#f76a67";
    public const string Wood = "#ab7b64";
    public const string Gold = "#fcdd00";
    public const string Stone = "#c7d1db";
}

public static class ResourceIcons
{
    public static readonly Dictionary<ResourceType, string> Map = new()
    {
        { ResourceType.Food,  Food },
        { ResourceType.Wood,  Wood },
        { ResourceType.Gold,  Gold },
        { ResourceType.Stone, Stone },
    };
    
    private const string RootDir = "res://Assets/UI/Icons";
    
    public const string Food = $"[img]{RootDir}/food.png[/img]";
    public const string Wood = $"[img]{RootDir}/wood.png[/img]";
    public const string Gold = $"[img]{RootDir}/gold.png[/img]";
    public const string Stone = $"[img]{RootDir}/stone.png[/img]";
}