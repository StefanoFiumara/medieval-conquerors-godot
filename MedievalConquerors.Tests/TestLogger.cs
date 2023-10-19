using System.Runtime.CompilerServices;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Extensions;
using Xunit.Abstractions;

namespace MedievalConquerors.Tests;

public class TestLogger : GameComponent, ILogger
{
    private readonly ITestOutputHelper _output;
    public LogLevel MinimumLogLevel { get; }

    public TestLogger(ITestOutputHelper output, LogLevel minimumLogLevel = LogLevel.Debug)
    {
        _output = output;
        MinimumLogLevel = minimumLogLevel;
    }
    public void Debug(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Debug, message, caller);
    public void Info(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Info, message, caller);
    public void Warn(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Warn, message, caller);
    public void Error(string message, [CallerFilePath] string caller = "") => Log(LogLevel.Error, message, caller);

    private void Log(LogLevel logLevel, string message, string callerFilePath)
    {
        if (logLevel.IsAbove(MinimumLogLevel))
        {
            _output.WriteLine($"{Prefix(callerFilePath)}{message}");
        }
    }

    // TODO: Share with GodotLogger (extension?)
    private string Prefix(string callerFilePath)
    {
        var prefix = "";

        if (!string.IsNullOrEmpty(callerFilePath))
        {
            var callerName = Path.GetFileNameWithoutExtension(callerFilePath);
            prefix = $"{callerName}\t======>\t";
        }

        return prefix;
    }
}