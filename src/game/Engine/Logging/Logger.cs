using System;
using System.Runtime.CompilerServices;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Logging;

public interface ILogger : IGameComponent
{
    void Debug(string message, [CallerFilePath] string caller = "");
    void Info(string message, [CallerFilePath] string caller = "");
    void Warn(string message, [CallerFilePath] string caller = "");
    void Error(string message, Exception ex = null, [CallerFilePath] string caller = "");
}

public class NullLogger : GameComponent, ILogger
{
    public void Debug(string message, string caller = null) { }
    public void Info(string message, string caller = null) { }
    public void Warn(string message, string caller = null) { }
    public void Error(string message, Exception ex = null, string caller = null) { }
}
