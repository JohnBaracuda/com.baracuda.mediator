using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Mediator
{
    public static class EventExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RaiseChecked(this EventBroadcast eventBroadcast)
        {
            if (eventBroadcast == null)
            {
                return;
            }
            eventBroadcast.Raise();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LambdaHandle AddLambda(this IReceiver receiver, Action lambda)
        {
            receiver.Add(lambda);
            return new LambdaHandle(() =>
            {
                receiver.Remove(lambda);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LambdaHandle AddLambda<T>(this IReceiver<T> receiver, Action<T> lambda)
        {
            receiver.Add(lambda);
            return new LambdaHandle(() =>
            {
                receiver.Remove(lambda);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LambdaHandle AddLambda<T1, T2>(this IReceiver<T1, T2> receiver, Action<T1, T2> lambda)
        {
            receiver.Add(lambda);
            return new LambdaHandle(() =>
            {
                receiver.Remove(lambda);
            });
        }
    }
}