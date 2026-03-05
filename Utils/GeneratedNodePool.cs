using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using System;
using System.Collections.Generic;

namespace BaseLib.Utils;

/// <summary>
/// Utility class for adding custom poolables to <seealso cref="NodePool"/>
/// specifically for poolables with a custom generation method rather than using a scene file
/// </summary>
public class GeneratedNodePool
{
    private static Dictionary<Type, INodePool> _pools = null;

    public static GeneratedNodePool<T> Init<T>(Func<T> constructor, int prewarmCount) where T : Node, IPoolable
    {
        Type typeFromHandle = typeof(T);
        
        _pools ??= (Dictionary<Type, INodePool>)AccessTools.DeclaredField(typeof(NodePool), "_pools").GetValue(null);

        if (_pools.TryGetValue(typeFromHandle, out INodePool _))
        {
            throw new InvalidOperationException($"Tried to init GeneratedNodePool for type {typeof(T)} but it's already initialized!");
        }

        GeneratedNodePool<T> nodePool = new(constructor, prewarmCount);
        _pools[typeFromHandle] = nodePool;
        return nodePool;
    }
}


public class GeneratedNodePool<T> : INodePool where T : Node, IPoolable
{
    private static Variant _nameStr = Variant.CreateFrom("name");
    private static Variant _callableStr = Variant.CreateFrom("callable");
    private static Variant _signalStr = Variant.CreateFrom("signal");

    private readonly Func<T> _constructor;

    private readonly List<T> _freeObjects = new List<T>();
    private readonly HashSet<T> _usedObjects = new HashSet<T>();

    public IReadOnlyList<T> DebugFreeObjects => _freeObjects;

    public GeneratedNodePool(Func<T> constructor, int prewarmCount = 0)
    {
        _constructor = constructor;
        for (int i = 0; i < prewarmCount; i++)
        {
            _freeObjects.Add(Instantiate());
        }
    }

    IPoolable INodePool.Get()
    {
        return Get();
    }

    void INodePool.Free(IPoolable poolable)
    {
        Free((T)poolable);
    }

    public T Get()
    {
        T val;
        if (_freeObjects.Count > 0)
        {
            List<T> freeObjects = _freeObjects;
            val = freeObjects[^1];
            _freeObjects.RemoveAt(_freeObjects.Count - 1);
        }
        else
        {
            val = Instantiate();
        }

        _usedObjects.Add(val);
        val.OnReturnedFromPool();
        return val;
    }

    public void Free(T obj)
    {
        if (!_usedObjects.Contains(obj))
        {
            if (_freeObjects.Contains(obj))
            {
                Log.Error($"Tried to free object {obj} ({obj.GetType()}) back to pool {typeof(GeneratedNodePool<T>)} but it's already been freed!");
            }
            else
            {
                Log.Error($"Tried to free object {obj} ({obj.GetType()}) back to pool {typeof(GeneratedNodePool<T>)} but it's not part of the pool!");
            }

            obj.QueueFreeSafelyNoPool();
        }
        else
        {
            DisconnectIncomingAndOutgoingSignals(obj);
            _usedObjects.Remove(obj);
            _freeObjects.Add(obj);
            obj.OnFreedToPool();
        }
    }

    private T Instantiate()
    {
        T val = _constructor();
        val.OnInstantiated();
        return val;
    }

    private void DisconnectIncomingAndOutgoingSignals(Node obj)
    {
        foreach (Godot.Collections.Dictionary signal4 in obj.GetSignalList())
        {
            StringName signal = signal4[_nameStr].AsStringName();
            foreach (Godot.Collections.Dictionary signalConnection in obj.GetSignalConnectionList(signal))
            {
                Callable callable = signalConnection[_callableStr].AsCallable();
                Signal signal2 = signalConnection[_signalStr].AsSignal();
                DisconnectSignal(callable, signal2);
            }
        }

        foreach (Godot.Collections.Dictionary incomingConnection in obj.GetIncomingConnections())
        {
            Callable callable2 = incomingConnection[_callableStr].AsCallable();
            Signal signal3 = incomingConnection[_signalStr].AsSignal();
            DisconnectSignal(callable2, signal3);
        }

        for (int i = 0; i < obj.GetChildCount(); i++)
        {
            DisconnectIncomingAndOutgoingSignals(obj.GetChild(i));
        }
    }

    private void DisconnectSignal(Callable callable, Signal signal)
    {
        GodotObject target = callable.Target;
        if (target == null && callable.Method == null)
        {
            return;
        }

        StringName name = signal.Name;
        Node node = target as Node;
        if (node == null || node.IsInsideTree())
        {
            GodotObject owner = signal.Owner;
            Node node2 = owner as Node;
            if (node != null && node.HasSignal(name) && node.IsConnected(name, callable))
            {
                node.Disconnect(name, callable);
            }
            else if (node2 != null && node2.HasSignal(name) && node2.IsConnected(name, callable))
            {
                node2.Disconnect(name, callable);
            }
        }
    }
}
