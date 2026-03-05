using BaseLib.Patches;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

public abstract class CustomPotionModel : PotionModel, ICustomModel
{
    public virtual bool AutoAdd => true;
    public CustomPotionModel()
    {
        if (AutoAdd) CustomContentDictionary.AddModel(GetType());
    }
}
