using MedievalConquerors.Engine.Logging;

namespace MedievalConquerors.Extensions;

public static class LoggerExtensions
{
    public static bool IsAbove(this LogLevel level, LogLevel minimum) => (int)level >= (int)minimum;
}
