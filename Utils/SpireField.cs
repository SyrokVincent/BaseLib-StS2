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
    private readonly ConditionalWeakTable<TKey, TVal?> _table = [];
    private readonly Func<TKey, TVal?> _defaultVal;

    public SpireField(Func<TVal?> defaultVal)
    {
        _defaultVal = _ => defaultVal();
    }

    public SpireField(Func<TKey, TVal?> defaultVal)
    {
        _defaultVal = defaultVal;
    }

    public TVal? this[TKey obj]
    {
        get => Get(obj);
        set => Set(obj, value);
    }

    public TVal? Get(TKey obj) {
        if (_table.TryGetValue(obj, out var result)) return result;

        _table.Add(obj, result = _defaultVal(obj));
        return result;
    }

    public void Set(TKey obj, TVal? val)
    {
        _table.AddOrUpdate(obj, val);
    }
}