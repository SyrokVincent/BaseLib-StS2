using BaseLib.Patches;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

public abstract class CustomRelicModel : RelicModel, ICustomModel
{
    public virtual bool AutoAdd => true;
    public CustomRelicModel()
    {
        if (AutoAdd) CustomContentDictionary.AddModel(GetType());
    }

    public virtual RelicModel GetUpgradeReplacement() => null;
}
