using System.IO;
using System.Runtime.CompilerServices;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.Logging;

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