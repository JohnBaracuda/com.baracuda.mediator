using System;

namespace Baracuda.Mediator
{
    public abstract class EventAsset : EventMediator, IReceiver
    {
        private protected readonly IBroadcast Event = new Broadcast();

        public void Add(Action listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique(Action listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action listener)
        {
            return Event.Contains(listener);
        }

        public void Clear()
        {
            Event.Clear();
        }

        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        public void Raise()
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise();
        }
    }

    public abstract class EventAsset<T> : EventMediator, IReceiver<T>
    {
        private protected readonly IBroadcast<T> Event = new Broadcast<T>();

        public void Add(Action<T> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique(Action<T> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T> listener)
        {
            return Event.Contains(listener);
        }

        public void Clear()
        {
            Event.Clear();
        }

        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        public void Raise(T value)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value);
        }
    }

    public abstract class EventAsset<T1, T2> : EventMediator, IReceiver<T1, T2>
    {
        private protected readonly IBroadcast<T1, T2> Event = new Broadcast<T1, T2>();

        public void Add(Action<T1, T2> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique(Action<T1, T2> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T1, T2> listener)
        {
            return Event.Contains(listener);
        }

        public void Clear()
        {
            Event.Clear();
        }

        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        public void Raise(T1 value1, T2 value2)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value1, value2);
        }
    }

    public abstract class EventAsset<T1, T2, T3> : EventMediator, IReceiver<T1, T2, T3>
    {
        private protected readonly IBroadcast<T1, T2, T3> Event = new Broadcast<T1, T2, T3>();

        public void Add(Action<T1, T2, T3> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique(Action<T1, T2, T3> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T1, T2, T3> listener)
        {
            return Event.Contains(listener);
        }

        public void Clear()
        {
            Event.Clear();
        }

        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        public void Raise(T1 value1, T2 value2, T3 value3)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value1, value2, value3);
        }
    }

    public abstract class EventAsset<T1, T2, T3, T4> : EventMediator, IReceiver<T1, T2, T3, T4>
    {
        private protected readonly IBroadcast<T1, T2, T3, T4> Event = new Broadcast<T1, T2, T3, T4>();

        public void Add(Action<T1, T2, T3, T4> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique(Action<T1, T2, T3, T4> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3, T4> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T1, T2, T3, T4> listener)
        {
            return Event.Contains(listener);
        }

        public void Clear()
        {
            Event.Clear();
        }

        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        public void Raise(T1 value1, T2 value2, T3 value3, T4 value4)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value1, value2, value3, value4);
        }
    }
}