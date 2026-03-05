using MegaCrit.Sts2.Core.ValueProps;

namespace BaseLib.Extensions;

public static class PublicPropExtensions
{
    public static bool IsPoweredAttack_(this ValueProp props)
    {
        if (props.HasFlag(ValueProp.Move))
        {
            return !props.HasFlag(ValueProp.Unpowered);
        }

        return false;
    }

    public static bool IsPoweredCardOrMonsterMoveBlock_(this ValueProp props)
    {
        if (props.HasFlag(ValueProp.Move))
        {
            return !props.HasFlag(ValueProp.Unpowered);
        }

        return false;
    }

    public static bool IsCardOrMonsterMove_(this ValueProp props)
    {
        return props.HasFlag(ValueProp.Move);
    }
}