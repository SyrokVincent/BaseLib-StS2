using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Linq;
using BaseLib.Patches;
using BaseLib.Patches.Content;

namespace BaseLib.Abstracts;

public abstract class CustomCardModel : CardModel, ICustomModel
{
    /// <summary>
    /// For convenience; can be manually overridden if necessary.
    /// </summary>
    public override bool GainsBlock => DynamicVars.Any((dynVar)=>dynVar.Value is BlockVar or CalculatedBlockVar);

    public CustomCardModel(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true) : base(baseCost, type, rarity, target, showInCardLibrary)
    {
        if (autoAdd) CustomContentDictionary.AddModel(GetType());
    }

    public virtual Texture2D? CustomFrame => null;
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Frame), MethodType.Getter)]
class CustomCardFrame
{
    [HarmonyPrefix]
    static bool UseAltTexture(CardModel __instance, ref Texture2D? __result)
    {
        if (__instance is CustomCardModel customCard)
        {
            __result = customCard.CustomFrame;
            if (__result != null) return false;

            if (__instance.Pool is CustomCardPoolModel customCardPool)
            {
                __result = customCardPool.CustomFrame(customCard);
                if (__result != null) return false;
            }
        }
        return true;
    }
}