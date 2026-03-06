using BaseLib.Patches;
using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseLib.Extensions;

public static class StringExtensions
{
    public static string RemovePrefix(this string id)
    {
        return id[(id.IndexOf(TypePrefix.PrefixSplitChar) + 1)..];
    }
}
