using System;
using System.Collections.Generic;
using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.Content;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.InitIds))]
public static class CustomContentDictionary
{
    private static readonly List<Type> _customTypes = [];
    private static readonly Dictionary<Type, Type> _poolTypes = [];
    static CustomContentDictionary()
    {
        _poolTypes.Add(typeof(CardPoolModel), typeof(CardModel));
        _poolTypes.Add(typeof(RelicPoolModel), typeof(RelicModel));
        _poolTypes.Add(typeof(PotionPoolModel), typeof(PotionModel));
    }


    public static void AddModel(Type modelType)
    {
        _customTypes.Add(modelType);

        PoolAttribute poolAttribute = modelType.GetCustomAttribute<PoolAttribute>()
            ?? throw new Exception($"Model {modelType.FullName} must be marked with a PoolAttribute to determine which pool to add it to.");

        if (!IsValidPool(modelType, poolAttribute.PoolType))
        {
            throw new Exception($"Model {modelType.FullName} is assigned to incorrect type of pool {poolAttribute.PoolType.FullName}.");
        }

        ModHelper.AddModelToPool(poolAttribute.PoolType, modelType);
    }
    private static bool IsValidPool(Type modelType, Type poolType)
    {
        var basePoolType = poolType.BaseType;
        while (basePoolType != null)
        {
            if (_poolTypes.TryGetValue(basePoolType, out var poolValueType))
            {
                return modelType.IsAssignableTo(poolValueType);
            }
            basePoolType = basePoolType.BaseType;
        }
        throw new Exception($"Model {modelType.FullName} is assigned to {poolType.FullName} which is not a valid pool type.");
    }


    [HarmonyPostfix]
    static void ConvertTypesToModels()
    {
        MainFile.Logger.Info($"Custom Models: {_customTypes.Count}");
    }
}

[HarmonyPatch(typeof(ModelDb), "AllSharedCardPools", MethodType.Getter)]
public class ModelDbSharedCardPoolsPatch
{
    private static readonly List<CardPoolModel> customSharedPools = [];

    [HarmonyPostfix]
    static IEnumerable<CardPoolModel> AddCustomPools(IEnumerable<CardPoolModel> __result)
    {
        return [.. __result, .. customSharedPools];
    }

    public static void Register(CustomCardPoolModel pool)
    {
        customSharedPools.Add(pool);
    }
}

[HarmonyPatch(typeof(ModelDb), "AllSharedRelicPools", MethodType.Getter)]
public class ModelDbSharedRelicPoolsPatch
{
    private static readonly List<RelicPoolModel> customSharedPools = [];

    [HarmonyPostfix]
    static IEnumerable<RelicPoolModel> AddCustomPools(IEnumerable<RelicPoolModel> __result)
    {
        return [.. __result, .. customSharedPools];
    }

    public static void Register(CustomRelicPoolModel pool)
    {
        customSharedPools.Add(pool);
    }
}

[HarmonyPatch(typeof(ModelDb), "AllSharedPotionPools", MethodType.Getter)]
public class ModelDbSharedPotionPoolsPatch
{
    private static readonly List<PotionPoolModel> customSharedPools = [];

    [HarmonyPostfix]
    static IEnumerable<PotionPoolModel> AddCustomPools(IEnumerable<PotionPoolModel> __result)
    {
        return [.. __result, .. customSharedPools];
    }

    public static void Register(CustomPotionPoolModel pool)
    {
        customSharedPools.Add(pool);
    }
}

/*
class CardPoolPatch
{
    internal static void Patch(Harmony harmony)
    {
        Type[] poolTypes = 
            [typeof(IroncladCardPool),
            typeof(SilentCardPool),
            typeof(DefectCardPool),
            typeof(RegentCardPool),
            typeof(NecrobinderCardPool),
            typeof(ColorlessCardPool),
            typeof(TokenCardPool),
            typeof(EventCardPool),
            typeof(QuestCardPool),
            typeof(StatusCardPool),
            typeof(CurseCardPool)
        ]; //CustomCardPoolModel generate method utilizes CustomContentDictionary

        foreach (var poolType in poolTypes)
        {
            var originalMethod = AccessTools.Method(poolType, "GenerateAllCards");
            var postfix = AccessTools.Method(typeof(CardPoolPatch), nameof(AdjustPool));
            harmony.Patch(originalMethod, postfix: new HarmonyMethod(postfix));
        }
    }

    static CardModel[] AdjustPool(CardModel[] __result, CardPoolModel __instance)
    {
        if (CustomContentDictionary.Cards.TryGetValue(__instance.GetType(), out var cards))
        {
            return [.. __result, .. cards];
        }
        return __result;
    }
}
class RelicPoolPatch
{
    internal static void Patch(Harmony harmony)
    {
        Type[] poolTypes =
            [typeof(SharedRelicPool),
            typeof(IroncladRelicPool),
            typeof(SilentRelicPool),
            typeof(DefectRelicPool),
            typeof(RegentRelicPool),
            typeof(NecrobinderRelicPool),
            typeof(EventRelicPool)
        ]; //CustomRelicPoolModel generate method utilizes CustomContentDictionary

        foreach (var poolType in poolTypes)
        {
            var originalMethod = AccessTools.Method(poolType, "GenerateAllRelics");
            var postfix = AccessTools.Method(typeof(RelicPoolPatch), nameof(AdjustPool));
            harmony.Patch(originalMethod, postfix: new HarmonyMethod(postfix));
        }
    }

    static IEnumerable<RelicModel> AdjustPool(IEnumerable<RelicModel> __result, RelicPoolModel __instance)
    {
        if (CustomContentDictionary.Relics.TryGetValue(__instance.GetType(), out var relics))
        {
            return [.. __result, .. relics];
        }
        return __result;
    }
}
class PotionPoolPatch
{
    internal static void Patch(Harmony harmony)
    {
        Type[] poolTypes =
            [typeof(SharedPotionPool),
            typeof(IroncladPotionPool),
            typeof(SilentPotionPool),
            typeof(DefectPotionPool),
            typeof(RegentPotionPool),
            typeof(NecrobinderPotionPool),
            typeof(EventPotionPool),
            typeof(TokenPotionPool)
        ]; //CustomPotionPoolModel generate method utilizes CustomContentDictionary

        foreach (var poolType in poolTypes)
        {
            var originalMethod = AccessTools.Method(poolType, "GenerateAllPotions");
            var postfix = AccessTools.Method(typeof(PotionPoolPatch), nameof(AdjustPool));
            harmony.Patch(originalMethod, postfix: new HarmonyMethod(postfix));
        }
    }

    static IEnumerable<PotionModel> AdjustPool(IEnumerable<PotionModel> __result, PotionPoolModel __instance)
    {
        if (CustomContentDictionary.Potions.TryGetValue(__instance.GetType(), out var potions))
        {
            return [.. __result, .. potions];
        }
        return __result;
    }
}*/