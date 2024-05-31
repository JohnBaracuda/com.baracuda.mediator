using System;

namespace Baracuda.Bedrock.Values
{
    public interface IObservable<TValue>
    {
        event Action<TValue> Changed;

        TValue Value { get; set; }

        TValue GetValue();

        void SetValue(TValue value);
    }
}