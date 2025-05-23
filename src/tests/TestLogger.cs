using System.Runtime.CompilerServices;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Extensions;
using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Tests;

public class TestLogger(ITestOutputHelper output, LogLevel minimumLogLevel = LogLevel.Debug) : GameComponent, ILogger
{
    public LogLevel MinimumLogLevel { get; } = minimumLogLevel;

    public void Debug(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Debug, message, caller);
    public void Info(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Info, message, caller);
    public void Warn(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Warn, message, caller);
    public void Error(string message, Exception? ex = null, [CallerFilePath] string caller = "") => Log(LogLevel.Error, message, caller);

    private void Log(LogLevel logLevel, string message, string callerFilePath)
    {
        if (logLevel.IsAbove(MinimumLogLevel))
        {
            output.WriteLine($"{Prefix(callerFilePath)}{message}");
        }
    }

    // TODO: Share with GodotLogger (extension?)
    private string Prefix(string callerFilePath)
    {
        var prefix = "";

        if (!string.IsNullOrEmpty(callerFilePath))
        {
            var callerName = Path.GetFileNameWithoutExtension(callerFilePath);
            prefix = $"{callerName}\t --> ";
        }

        return prefix;
    }
}
