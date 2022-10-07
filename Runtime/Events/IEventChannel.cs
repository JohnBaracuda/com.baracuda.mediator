using System;

namespace Baracuda.Mediator.Events
{
    public interface IEventChannel
    {
        public void AddListener(Action listener);
        public void AddUniqueListener(Action listener);
        public void RemoveListener(Action listener);
        public void Raise();
    }

    public interface IEventChannel<T>
    {
        public void AddListener(Action<T> listener);
        public void AddUniqueListener(Action<T> listener);
        public void RemoveListener(Action<T> listener);
        public void Raise(in T value);
    }

    public interface IEventChannel<T1, T2>
    {
        public void AddListener(Action<T1, T2> listener);
        public void AddUniqueListener(Action<T1, T2> listener);
        public void RemoveListener(Action<T1, T2> listener);
        public void Raise(in T1 first, in T2 second);
    }

    public interface IEventChannel<T1, T2, T3>
    {
        public void AddListener(Action<T1, T2, T3> listener);
        public void AddUniqueListener(Action<T1, T2, T3> listener);
        public void RemoveListener(Action<T1, T2, T3> listener);
        public void Raise(in T1 first, in T2 second, in T3 third);
    }

    public interface IEventChannel<T1, T2, T3, T4>
    {
        public void AddListener(Action<T1, T2, T3, T4> listener);
        public void AddUniqueListener(Action<T1, T2, T3, T4> listener);
        public void RemoveListener(Action<T1, T2, T3, T4> listener);
        public void Raise(in T1 first, in T2 second, in T3 third, in T4 forth);
    }
}