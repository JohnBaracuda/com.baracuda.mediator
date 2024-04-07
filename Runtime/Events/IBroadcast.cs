namespace Baracuda.Mediator.Events
{
    public interface IBroadcast : IReceiver
    {
        /// <summary> Raise the event </summary>
        public void Raise();

        /// <summary> Raise the event with exception handling </summary>
        public void RaiseCritical();
    }

    public interface IBroadcast<T> : IReceiver<T>
    {
        /// <summary> Raise the event </summary>
        public void Raise(T arg);

        public void RaiseCritical(T arg);
    }

    public interface IBroadcast<T1, T2> : IReceiver<T1, T2>
    {
        /// <summary> Raise the event </summary>
        public void Raise(T1 value1, T2 value2);

        /// <summary> Raise the event with exception handling </summary>
        public void RaiseCritical(T1 value1, T2 value2);
    }

    public interface IBroadcast<T1, T2, T3> : IReceiver<T1, T2, T3>
    {
        /// <summary> Raise the event </summary>
        public void Raise(T1 first, T2 second, T3 third);

        /// <summary> Raise the event with exception handling </summary>
        public void RaiseCritical(T1 first, T2 second, T3 third);
    }

    public interface IBroadcast<T1, T2, T3, T4> : IReceiver<T1, T2, T3, T4>
    {
        /// <summary> Raise the event </summary>
        public void Raise(T1 first, T2 second, T3 third, T4 forth);

        /// <summary> Raise the event with exception handling </summary>
        public void RaiseCritical(T1 first, T2 second, T3 third, T4 forth);
    }
}