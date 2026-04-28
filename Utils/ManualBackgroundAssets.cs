using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;

namespace BaseLib.Utils;

/// <summary>
/// Instead of creating the Asset Paths through reading a directory they are instead inserted directly as strings.
/// </summary>
public class ManualBackgroundAssets : BackgroundAssets
{
    private static readonly FieldInfo BgSceneField = AccessTools.Field(typeof(BackgroundAssets), "<BackgroundScenePath>k__BackingField");
    private static readonly FieldInfo BgLayersField = AccessTools.Field(typeof(BackgroundAssets), "<BgLayers>k__BackingField");
    private static readonly FieldInfo FgLayerField = AccessTools.Field(typeof(BackgroundAssets), "<FgLayer>k__BackingField");

    private const string FAKE_KEY = "glory";
    
    public ManualBackgroundAssets() : base(FAKE_KEY, Rng.Chaotic) { }
    
    public ManualBackgroundAssets((string ScenePath, List<string> BackgroundLayerPaths, string? ForegroundLayerPath) customBackgroundAssets) : base(FAKE_KEY, Rng.Chaotic)
    {
        BgSceneField.SetValue(this, customBackgroundAssets.ScenePath);
        BgLayersField.SetValue(this, customBackgroundAssets.BackgroundLayerPaths);
        FgLayerField.SetValue(this, customBackgroundAssets.ForegroundLayerPath);
    }
    
    public ManualBackgroundAssets(string backgroundScenePath, List<string> backgroundLayers, string foreGroundLayer) : base(FAKE_KEY, Rng.Chaotic)
    {
        BgSceneField.SetValue(this, backgroundScenePath);
        BgLayersField.SetValue(this, backgroundLayers);
        FgLayerField.SetValue(this, foreGroundLayer);
    }
    
    public void SetBackgroundScene(string scenePath) => BgSceneField.SetValue(this, scenePath);
    public void SetBackgroundLayer(string layerPath) => BgLayersField.SetValue(this, layerPath);
    public void SetForegroundLayer(string layerPath) => FgLayerField.SetValue(this, layerPath);
}
