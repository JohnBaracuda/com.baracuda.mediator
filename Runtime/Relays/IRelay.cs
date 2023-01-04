using System;

namespace Baracuda.Mediator.Relays
{
    public interface IRelay
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action listener);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<out T>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T> listener);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<out T1, out T2>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T1, T2> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T1, T2> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T1, T2> listener);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T1, T2> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<out T1, out T2, out T3>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T1, T2, T3> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T1, T2, T3> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T1, T2, T3> listener);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T1, T2, T3> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<out T1, out T2, out T3, out T4>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T1, T2, T3, T4> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T1, T2, T3, T4> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T1, T2, T3, T4> listener);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T1, T2, T3, T4> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }
}