using System;

namespace BaseLib.Extensions;

public static class TypePrefix
{
    public const char PREFIX_SPLIT_CHAR = '-';
    public static string GetPrefix(this Type t)
    {
        return $"{t.Namespace[..t.Namespace.IndexOf('.')].ToUpperInvariant()}{PREFIX_SPLIT_CHAR}";
    }
}
