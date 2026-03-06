using System;

namespace BaseLib.Extensions;

public static class TypePrefix
{
    public const char PrefixSplitChar = '-';
    public static string GetPrefix(this Type t)
    {
        return t.Namespace == null ? "" : 
            $"{t.Namespace[..t.Namespace.IndexOf('.')].ToUpperInvariant()}{PrefixSplitChar}";
    }
}
