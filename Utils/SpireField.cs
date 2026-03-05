using System;
using System.Runtime.CompilerServices;

namespace BaseLib.Utils;

/// <summary>
/// A basic wrapper around <seealso cref="ConditionalWeakTable{TKey, TValue}"/> for convenience.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TVal"></typeparam>
/// <param name="defaultVal"></param>
public class SpireField<TKey, TVal> where TKey : class where TVal : class
{
    private readonly ConditionalWeakTable<TKey, TVal> table = [];
    private Func<TKey, TVal> _defaultVal;

    public SpireField(Func<TVal> defaultVal)
    {
        _defaultVal = (key) => defaultVal();
    }

    public SpireField(Func<TKey, TVal> defaultVal)
    {
        _defaultVal = defaultVal;
    }

    public TVal this[TKey obj]
    {
        get => Get(obj);
        set => Set(obj, value);
    }

    public TVal Get(TKey obj) {
        if (obj == null) return null;
        if (table.TryGetValue(obj, out var result)) return result;

        table.Add(obj, result = _defaultVal(obj));
        return result;
    }

    public void Set(TKey obj, TVal val) {
        table.AddOrUpdate(obj, val);
    }
}