using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Extensions;

public static class DynamicVarSetExtensions
{
    /// <summary>
    /// Get a power var initialized with its default name.
    /// </summary>
    public static DynamicVar Power<T>(this DynamicVarSet vars) where T : PowerModel
    {
        return vars[typeof(T).Name];
    }
}