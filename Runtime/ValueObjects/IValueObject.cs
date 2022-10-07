using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Mediator.ValueObjects
{
    public interface IValueObject<TValue>
    {
        public TValue Value { get; set; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddListener(Action<TValue> listener);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveListener(Action<TValue> listener);
    }
}