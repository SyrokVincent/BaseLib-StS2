using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;

namespace BaseLib.Extensions;

public static class ActModelExtensions
{
    public static int ActNumber(this ActModel actModel)
    {
        //if (actModel is CustomActModel customAct) return customAct.ActNumber;
        return actModel switch
        {
            Overgrowth or Underdocks => 1,
            Hive => 2,
            Glory => 3,
            _ => -1
        };
    }
}