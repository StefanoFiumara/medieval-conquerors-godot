using System.IO;
using System.Runtime.CompilerServices;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Engine;

public static class ColorExtensions
{
    public static string Black(this string msg) => WrapInColor(msg, "black");
    public static string Red(this string msg) => WrapInColor(msg, "red");
    public static string Green(this string msg) => WrapInColor(msg, "green");
    public static string Yellow(this string msg) => WrapInColor(msg, "yellow");
    public static string Blue(this string msg) => WrapInColor(msg, "blue");
    public static string Magenta(this string msg) => WrapInColor(msg, "magenta");
    public static string Pink(this string msg) => WrapInColor(msg, "pink");
    public static string Purple(this string msg) => WrapInColor(msg, "purple");
    public static string Cyan(this string msg) => WrapInColor(msg, "cyan");
    public static string White(this string msg) => WrapInColor(msg, "white");
    public static string Orange(this string msg) => WrapInColor(msg, "orange");
    public static string Gray(this string msg) => WrapInColor(msg, "gray");

    private static string WrapInColor(string msg, string color) => $"[color={color}]{msg}[/color]";
}

public class GodotLogger : GameComponent, ILogger
{
    public LogLevel MinimumLogLevel { get; }

    public GodotLogger(LogLevel minimumLogLevel)
    {
        MinimumLogLevel = minimumLogLevel;
    }
	
    private void Log(LogLevel logLevel, string message, string callerFilePath)
    {
        if (!logLevel.IsAbove(MinimumLogLevel)) return;

        var prefix = "";

        if (!string.IsNullOrEmpty(callerFilePath))
        {
            var callerName = Path.GetFileNameWithoutExtension(callerFilePath);
            prefix = $"{callerName}\t======>\t";
        }
        
        
        switch (logLevel)
        {
            case LogLevel.Debug:
                GD.PrintRich($"{prefix}{message}".Cyan());
                break;
            case LogLevel.Info:
                GD.PrintRich($"{prefix}{message}".Green());
                break;
            case LogLevel.Warn:
                GD.PrintRich($"{prefix}{message}".Yellow());
                break;
            case LogLevel.Error:
                GD.PrintRich($"{prefix}{message}".Red());
                break;
            case LogLevel.None:
            default:
                break;
        }
    }

    public void Debug(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Debug, message, caller);
    public void Info(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Info, message, caller);
    public void Warn(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Warn, message, caller);
    public void Error(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Error, message, caller);
}