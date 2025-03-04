using System.IO;
using System.Runtime.CompilerServices;
using Godot;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.Logging;

public class GodotLogger(LogLevel minimumLogLevel) : GameComponent, ILogger
{
    public LogLevel MinimumLogLevel { get; } = minimumLogLevel;

    private void Log(LogLevel logLevel, string message, string callerFilePath)
    {
        if (!logLevel.IsAbove(MinimumLogLevel)) return;

        var prefix = $"{logLevel.ToString().ToUpper()}: ";

        if (!string.IsNullOrEmpty(callerFilePath))
        {
            var callerName = Path.GetFileNameWithoutExtension(callerFilePath);
            prefix += $"{callerName}\t --> ";
        }

        switch (logLevel)
        {
            case LogLevel.Debug:
                GD.PrintRich($"{prefix}{message}".Cyan());
                break;
            case LogLevel.Info:
                GD.PrintRich($"{prefix}{message}");
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
