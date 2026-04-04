using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace BaseLib.Abstracts;

/// <summary>
/// Model that will passively receive hooks at all times.
/// </summary>
public abstract class CustomSingletonModel : SingletonModel, ICustomModel
{
    private static readonly MethodInfo? SubscribeCombat = typeof(ModHelper).GetMethod("SubscribeForCombatStateHooks");
    private static readonly MethodInfo? SubscribeRunState = typeof(ModHelper).GetMethod("SubscribeForRunStateHooks");

    private static readonly Type? RunHookSubDelegate =
        Type.GetType("MegaCrit.Sts2.Core.Modding.RunHookSubscriptionDelegate, sts2");
    private static readonly Type? CombatHookSubDelegate =
        Type.GetType("MegaCrit.Sts2.Core.Modding.CombatHookSubscriptionDelegate, sts2");
    
    /// <summary>
    /// This property seems effectively unused; it is set anyways in case of future changes.
    /// </summary>
    public override bool ShouldReceiveCombatHooks { get; }

    public CustomSingletonModel(bool receiveCombatHooks, bool receiveRunHooks)
    {
        ShouldReceiveCombatHooks = receiveCombatHooks;

        if (SubscribeCombat == null || SubscribeRunState == null)
        {
            BaseLibMain.Logger.Warn($"CustomSingleton {GetType().FullName} created; not supported on current game branch");
            return;
        }

        if (receiveRunHooks)
        {
            SubscribeRunState.Invoke(null, [Id.Entry, Delegate.CreateDelegate(RunHookSubDelegate!, this, typeof(CustomSingletonModel).GetMethod("RunSubModels")!)]);
        }
        if (receiveCombatHooks)
        {
            SubscribeCombat.Invoke(null, [Id.Entry, Delegate.CreateDelegate(CombatHookSubDelegate!, this, typeof(CustomSingletonModel).GetMethod("CombatSubModels")!)]);
        }
    }

    public IEnumerable<AbstractModel> RunSubModels(RunState runState)
    {
        return [this];
    }
    public IEnumerable<AbstractModel> CombatSubModels(CombatState combatState)
    {
        return [this];
    }
}