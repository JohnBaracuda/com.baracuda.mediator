using System;

namespace Baracuda.Mediator
{
    public abstract class EventAsset : EventMediator, IReceiver
    {
        private protected readonly IBroadcast Event = new Broadcast();

        public void Add(Action listener)
        {
            Event.Add(listener);
        }

        public bool AddUnique(Action listener)
        {
            return Event.AddUnique(listener);
        }

        public bool Remove(Action listener)
        {
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
    }

    public abstract class EventAsset<T> : EventMediator, IReceiver<T>
    {
        private protected readonly IBroadcast<T> Event = new Broadcast<T>();

        /// <inheritdoc />
        public void Add(Action<T> listener)
        {
            Event.Add(listener);
        }

        public bool AddUnique(Action<T> listener)
        {
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T> listener)
        {
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
    }

    public abstract class EventAsset<T1, T2> : EventMediator, IReceiver<T1, T2>
    {
        private protected readonly IBroadcast<T1, T2> Event = new Broadcast<T1, T2>();

        public void Add(Action<T1, T2> listener)
        {
            Event.Add(listener);
        }

        public bool AddUnique(Action<T1, T2> listener)
        {
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2> listener)
        {
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
    }

    public abstract class EventAsset<T1, T2, T3> : EventMediator, IReceiver<T1, T2, T3>
    {
        private protected readonly IBroadcast<T1, T2, T3> Event = new Broadcast<T1, T2, T3>();

        public void Add(Action<T1, T2, T3> listener)
        {
            Event.Add(listener);
        }

        public bool AddUnique(Action<T1, T2, T3> listener)
        {
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3> listener)
        {
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
    }

    public abstract class EventAsset<T1, T2, T3, T4> : EventMediator, IReceiver<T1, T2, T3, T4>
    {
        private protected readonly IBroadcast<T1, T2, T3, T4> Event = new Broadcast<T1, T2, T3, T4>();

        public void Add(Action<T1, T2, T3, T4> listener)
        {
            Event.Add(listener);
        }

        public bool AddUnique(Action<T1, T2, T3, T4> listener)
        {
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3, T4> listener)
        {
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
    }
}