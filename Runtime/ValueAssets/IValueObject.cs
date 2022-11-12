using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Mediator.ValueAssets
{
    public interface IValueObject<TValue>
    {
        public TValue Value { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<TValue> listener);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddUnique(Action<TValue> listener);
    }
}