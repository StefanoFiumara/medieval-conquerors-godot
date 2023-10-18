namespace MedievalConquerors.Engine.Logging;

public static class LoggerExtensions
{
    public static bool IsAbove(this LogLevel level, LogLevel minimum) => (int)level >= (int)minimum;
}