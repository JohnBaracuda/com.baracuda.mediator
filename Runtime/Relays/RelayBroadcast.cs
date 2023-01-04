using System.Runtime.CompilerServices;

namespace Baracuda.Mediator.Relays
{
    public class RelayBroadcast : Relay, IRelayBroadcast
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise()
        {
            RaiseInternal();
        }
    }

    public class RelayBroadcast<T> : Relay<T>, IRelayBroadcast<T>
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(in T value)
        {
            RaiseInternal(value);
        }
    }

    public class RelayBroadcast<T1, T2> : Relay<T1, T2>, IRelayBroadcast<T1, T2>
    {
        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second)
        {
            RaiseInternal(first, second);
        }
    }

    public class RelayBroadcast<T1, T2, T3> : Relay<T1, T2, T3>, IRelayBroadcast<T1, T2, T3>
    {
        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second, in T3 third)
        {
            RaiseInternal(first, second, third);
        }
    }

    public class RelayBroadcast<T1, T2, T3, T4> : Relay<T1, T2, T3, T4>, IRelayBroadcast<T1, T2, T3, T4>
    {
        /// <summary> Raise the relay event </summary>
        public void Raise(in T1 first, in T2 second, in T3 third, in T4 forth)
        {
            RaiseInternal(first, second, third, forth);
        }
    }
}
