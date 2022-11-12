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

        /// <summary> Raise the relay event </summary>
        public void Raise();

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<T>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T> listener);

        /// <summary> Raise the relay event </summary>
        public void Raise(in T arg);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<T1, T2>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T1, T2> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T1, T2> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T1, T2> listener);

        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T1, T2> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<T1, T2, T3>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T1, T2, T3> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T1, T2, T3> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T1, T2, T3> listener);

        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second, in T3 third);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T1, T2, T3> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }

    public interface IRelay<T1, T2, T3, T4>
    {
        /// <summary> Add a listener to the relay event </summary>
        public void Add(Action<T1, T2, T3, T4> listener);

        /// <summary> Add a listener to the relay event if it is not already added </summary>
        public bool AddUnique(Action<T1, T2, T3, T4> listener);

        /// <summary> Remove a listener from the relay event </summary>
        public bool Remove(Action<T1, T2, T3, T4> listener);

        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second, in T3 third, in T4 forth);

        /// <summary> Check if the relay event contains the passed listener </summary>
        public bool Contains(Action<T1, T2, T3, T4> listener);

        /// <summary> Remove all listener from the relay event </summary>
        public void Clear();
    }
}