using System;

namespace MedievalConquerors.Engine.Events;

public static class GameEvent
{
    public static string Validate (Type t) => $"{t.Name}.Validate";
    public static string Prepare (Type t) => $"{t.Name}.Prepare";
    public static string Perform (Type t) => $"{t.Name}.Perform";
    public static string Cancel(Type t) => $"{t.Name}.Cancel";

    public static string Validate<T>() => Validate(typeof(T));
    public static string Prepare<T>() => Prepare(typeof(T));
    public static string Perform<T>() => Perform(typeof(T));
    public static string Cancel<T>() => Cancel(typeof(T));
}