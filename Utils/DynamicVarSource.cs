using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Utils;

/// <summary>
/// Represents a class that contains a DynamicVarSet.
/// </summary>
public sealed class DynamicVarSource()
{
    public required DynamicVarSet DynamicVars { get; init; }
    public required Creature Owner { get; init; }
    public CardModel? Card { get; init; }
    public RelicModel? Relic { get; init; }
    public PowerModel? Power { get; init; }
    
    
    
    public static implicit operator DynamicVarSource(CardModel card)
    {
        return new DynamicVarSource
        {
            DynamicVars = card.DynamicVars,
            Owner = card.Owner.Creature,
            Card = card
        };
    }
    
    public static implicit operator DynamicVarSource(RelicModel relic)
    {
        return new DynamicVarSource
        {
            DynamicVars = relic.DynamicVars,
            Owner = relic.Owner.Creature,
            Relic = relic
        };
    }
    
    public static implicit operator DynamicVarSource(PowerModel power)
    {
        return new DynamicVarSource
        {
            DynamicVars = power.DynamicVars,
            Owner = power.Owner,
            Power = power
        };
    }
}