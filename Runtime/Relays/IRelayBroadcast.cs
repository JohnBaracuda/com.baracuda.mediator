namespace Baracuda.Mediator.Relays
{
    public interface IRelayBroadcast : IRelay
    {
        /// <summary> Raise the relay event </summary>
        public void Raise();
    }

     public interface IRelayBroadcast<T> : IRelay<T>
    {
        /// <summary> Raise the relay event </summary>
        public void Raise(in T arg);
    }

    public interface IRelayBroadcast<T1, T2> : IRelay<T1, T2>
    {
        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second);
    }

    public interface IRelayBroadcast<T1, T2, T3> : IRelay<T1, T2, T3>
    {
        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second, in T3 third);
    }

    public interface IRelayBroadcast<T1, T2, T3, T4> : IRelay<T1, T2, T3, T4>
    {
        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second, in T3 third, in T4 forth);
    }
}