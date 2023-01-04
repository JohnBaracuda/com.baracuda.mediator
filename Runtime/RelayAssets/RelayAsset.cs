using Baracuda.Mediator.Relays;
using Baracuda.Utilities.Inspector;
using System;
using UnityEngine;

namespace Baracuda.Mediator.RelayAssets
{
    public abstract class RelayAsset : ScriptableObject, IRelay
    {
        private readonly IRelayBroadcast _relay = new RelayBroadcast();

        public void Add(Action listener)
        {
            _relay.Add(listener);
        }

        public bool AddUnique(Action listener)
        {
            return _relay.AddUnique(listener);
        }

        public bool Remove(Action listener)
        {
            return _relay.Remove(listener);
        }

        [Button]
        public void Raise()
        {
            _relay.Raise();
        }

        public bool Contains(Action listener)
        {
            return _relay.Contains(listener);
        }

        public void Clear()
        {
            _relay.Clear();
        }
    }

    public abstract class RelayAsset<T> : ScriptableObject, IRelay<T>
    {
        private readonly IRelayBroadcast<T> _relay = new RelayBroadcast<T>();

        public void Add(Action<T> listener)
        {
            _relay.Add(listener);
        }

        public bool AddUnique(Action<T> listener)
        {
            return _relay.AddUnique(listener);
        }

        public bool Remove(Action<T> listener)
        {
            return _relay.Remove(listener);
        }

        public void Raise(in T arg)
        {
            _relay.Raise(arg);
        }

        public bool Contains(Action<T> listener)
        {
            return _relay.Contains(listener);
        }

        public void Clear()
        {
            _relay.Clear();
        }
    }

    public abstract class RelayAsset<T1, T2> : ScriptableObject, IRelay<T1, T2>
    {
        private readonly IRelayBroadcast<T1, T2> _relay = new RelayBroadcast<T1, T2>();

        public void Add(Action<T1, T2> listener)
        {
            _relay.Add(listener);
        }

        public bool AddUnique(Action<T1, T2> listener)
        {
            return _relay.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2> listener)
        {
            return _relay.Remove(listener);
        }

        public void Raise(in T1 value1, in T2 value2)
        {
            _relay.Raise(value1, value2);
        }

        public bool Contains(Action<T1, T2> listener)
        {
            return _relay.Contains(listener);
        }

        public void Clear()
        {
            _relay.Clear();
        }
    }

    public abstract class RelayAsset<T1, T2, T3> : ScriptableObject, IRelay<T1, T2, T3>
    {
        private readonly IRelayBroadcast<T1, T2, T3> _relay = new RelayBroadcast<T1, T2, T3>();

        public void Add(Action<T1, T2, T3> listener)
        {
            _relay.Add(listener);
        }

        public bool AddUnique(Action<T1, T2, T3> listener)
        {
            return _relay.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3> listener)
        {
            return _relay.Remove(listener);
        }

        public void Raise(in T1 value1, in T2 value2, in T3 value3)
        {
            _relay.Raise(value1, value2, value3);
        }

        public bool Contains(Action<T1, T2, T3> listener)
        {
            return _relay.Contains(listener);
        }

        public void Clear()
        {
            _relay.Clear();
        }
    }

    public abstract class RelayAsset<T1, T2, T3, T4> : ScriptableObject, IRelay<T1, T2, T3, T4>
    {
        private readonly IRelayBroadcast<T1, T2, T3, T4> _relay = new RelayBroadcast<T1, T2, T3, T4>();

        public void Add(Action<T1, T2, T3, T4> listener)
        {
            _relay.Add(listener);
        }

        public bool AddUnique(Action<T1, T2, T3, T4> listener)
        {
            return _relay.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3, T4> listener)
        {
            return _relay.Remove(listener);
        }

        public void Raise(in T1 value1, in T2 value2, in T3 value3, in T4 value4)
        {
            _relay.Raise(value1, value2, value3, value4);
        }

        public bool Contains(Action<T1, T2, T3, T4> listener)
        {
            return _relay.Contains(listener);
        }

        public void Clear()
        {
            _relay.Clear();
        }
    }
}