using System.Collections.Concurrent;
using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils.Attributes;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.Content;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.GetEntry))]
public class PrefixIdPatch
{
    private static readonly ConcurrentDictionary<Type, string> IdCache = new();

    [HarmonyPostfix]
    static void AdjustID(ref string __result, Type type)
    {
        if (IdCache.TryGetValue(type, out var cachedId))
        {
            __result = cachedId;
            return;
        }
        
        var attr = type.GetCustomAttribute<CustomIDAttribute>();
        if (attr != null)
        {
            IdCache[type] = attr.ID;
            __result = attr.ID;
            return;
        }
        
        if (type.IsAssignableTo(typeof(ICustomModel)))
        {
            IdCache[type] = type.GetPrefix() + __result;
            __result = IdCache[type];
            return;
        }

        IdCache[type] = __result; //No modification
    }
}
