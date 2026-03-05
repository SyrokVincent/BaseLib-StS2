using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseLib.Extensions;

public static class MethodInfoExtensions
{
    public static Type StateMachineType(this MethodInfo methodInfo)
    {
        var stateMachineAttribute = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>();
        if (stateMachineAttribute == null) throw new ArgumentException($"MethodInfo {methodInfo.FullDescription()} is not an async method");
        return stateMachineAttribute.StateMachineType;
    }
}
