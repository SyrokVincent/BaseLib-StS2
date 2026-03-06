using BaseLib.Patches;
using BaseLib.Patches.Content;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

//All card pools must either be a character pool or a shared pool, otherwise they will not be found.
//Character pools are found from CharacterModel.CardPool
//Shared pools are defined in ModelDb.AllSharedCardPools
public abstract class CustomCardPoolModel : CardPoolModel, ICustomModel
{
    public CustomCardPoolModel()
    {
        if (IsShared) ModelDbSharedCardPoolsPatch.Register(this);
    }

    /// <summary>
    /// Back image of a card. Not required; basegame cards all use the same frames, colored using a shader.
    /// Ancient rarity uses a separate frame that ignores this logic.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public virtual Texture2D? CustomFrame(CustomCardModel card)
    {
        return null;
    }
    /// <summary>
    /// Material is a shader material that will be applied to the frame texture. A custom frame can be used by overriding <seealso cref="CustomFrame"/>.
    /// Override this only if you have a custom shadermaterial.
    /// If not overridden, a custom material will be automatically defined using <seealso cref="ShaderColor"/>.
    /// </summary>
    public override string CardFrameMaterialPath => "card_frame_red";

    /// <summary>
    /// Used for HSV values of material shader.
    /// HSV can instead be overridden directly.
    /// </summary>
    public virtual Color ShaderColor => new("FFFFFF");
    public virtual float H => ShaderColor.H;
    public virtual float S => ShaderColor.S;
    public virtual float V => ShaderColor.V;

    public override CardModel[] GenerateAllCards() => []; //Content added through ModHelper.ConcatModelsFromMods

    public virtual bool IsShared => false;
}

[HarmonyPatch(typeof(CardPoolModel), "FrameMaterial", MethodType.Getter)]
class CustomCardPoolMaterialPatch
{
    [HarmonyPrefix]
    static bool UseCustomMaterial(CardPoolModel __instance, ref Material __result)
    {
        if (__instance is CustomCardPoolModel customPool) {
            if (!customPool.CardFrameMaterialPath.Equals("card_frame_red")) return true;

            __result = ShaderUtils.GenerateHsv(customPool.H, customPool.S, customPool.V);
            return false;
        }
        return true;
    }
}
