using System.Runtime.CompilerServices;

namespace Baracuda.Mediator.Events
{
    public class Broadcast : Receiver, IBroadcast
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise()
        {
            RaiseInternal();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical()
        {
            RaiseCriticalInternal();
        }
    }

    public class Broadcast<T> : Receiver<T>, IBroadcast<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise(T value)
        {
            RaiseInternal(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T value)
        {
            RaiseCriticalInternal(value);
        }
    }

    public class Broadcast<T1, T2> : Receiver<T1, T2>, IBroadcast<T1, T2>
    {
        public void Raise(T1 first, T2 second)
        {
            RaiseInternal(first, second);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T1 first, T2 second)
        {
            RaiseCriticalInternal(first, second);
        }
    }

    public class Broadcast<T1, T2, T3> : Receiver<T1, T2, T3>, IBroadcast<T1, T2, T3>
    {
        public void Raise(T1 first, T2 second, T3 third)
        {
            RaiseInternal(first, second, third);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T1 first, T2 second, T3 third)
        {
            RaiseCriticalInternal(first, second, third);
        }
    }

    public class Broadcast<T1, T2, T3, T4> : Receiver<T1, T2, T3, T4>, IBroadcast<T1, T2, T3, T4>
    {
        public void Raise(T1 first, T2 second, T3 third, T4 forth)
        {
            RaiseInternal(first, second, third, forth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCritical(T1 first, T2 second, T3 third, T4 forth)
        {
            RaiseCriticalInternal(first, second, third, forth);
        }
    }
}