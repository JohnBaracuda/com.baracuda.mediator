using System.Runtime.CompilerServices;

namespace Baracuda.Mediator
{
    public class Broadcast : Receiver, IBroadcast
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise()
        {
            RaiseInternal();
        }
    }

    public class Broadcast<T> : Receiver<T>, IBroadcast<T>
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T value)
        {
            RaiseInternal(value);
        }
    }

    public class Broadcast<T1, T2> : Receiver<T1, T2>, IBroadcast<T1, T2>
    {
        /// <summary> Raise the event </summary>
        public void Raise(T1 value1, T2 value2)
        {
            RaiseInternal(value1, value2);
        }
    }

    public class Broadcast<T1, T2, T3> : Receiver<T1, T2, T3>, IBroadcast<T1, T2, T3>
    {
        /// <summary> Raise the event </summary>
        public void Raise(T1 first, T2 second, T3 third)
        {
            RaiseInternal(first, second, third);
        }
    }

    public class Broadcast<T1, T2, T3, T4> : Receiver<T1, T2, T3, T4>, IBroadcast<T1, T2, T3, T4>
    {
        /// <summary> Raise the event </summary>
        public void Raise(T1 first, T2 second, T3 third, T4 forth)
        {
            RaiseInternal(first, second, third, forth);
        }
    }
}
