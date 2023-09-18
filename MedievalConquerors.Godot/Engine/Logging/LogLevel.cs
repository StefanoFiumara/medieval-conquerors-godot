namespace MedievalConquerors.Engine.Logging;

public enum LogLevel
{
    Debug, 
    Info, 
    Warn, 
    Error,
    None
}

public static class LoggerExtensions
{
    public static bool IsAbove(this LogLevel level, LogLevel minimum) => (int)level >= (int)minimum;
}