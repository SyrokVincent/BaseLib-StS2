using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseLib.Extensions;


public static class HarmonyExtensions
{
    public static void PatchAsyncMoveNext(this Harmony harmony, MethodInfo asyncMethod, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
    {
        var moveNextMethod = asyncMethod.StateMachineType().GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);

        harmony.Patch(moveNextMethod, prefix, postfix, transpiler, finalizer);
    }
    public static void PatchAsyncMoveNext(this Harmony harmony, MethodInfo asyncMethod, out Type stateMachineType, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
    {
        var stateMachineAttribute = asyncMethod.GetCustomAttribute<AsyncStateMachineAttribute>();
        if (stateMachineAttribute == null) throw new ArgumentException($"MethodInfo {asyncMethod.FullDescription()} passed to PatchAsync is not an async method");
        stateMachineType = stateMachineAttribute.StateMachineType;
        var moveNextMethod = stateMachineType.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);

        harmony.Patch(moveNextMethod, prefix, postfix, transpiler, finalizer);
    }
}
