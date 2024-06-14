using Baracuda.Bedrock.Events;
using JetBrains.Annotations;
using System;

namespace Baracuda.Bedrock.Values
{
    public class Observable<TValue> : IObservable<TValue>
    {
        private TValue _value;
        private readonly Broadcast<TValue> _changed = new();

        public event Action<TValue> Changed
        {
            add
            {
                _changed.Add(value);
                value(_value);
            }
            remove => _changed.Remove(value);
        }

        [PublicAPI]
        public TValue Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        [PublicAPI]
        public TValue GetValue()
        {
            return _value;
        }

        [PublicAPI]
        public void SetValue(TValue value)
        {
            _value = value;
            _changed.Raise(value);
        }

        [PublicAPI]
        public bool TryGetValue(out TValue value)
        {
            if (_value != null)
            {
                value = _value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        [PublicAPI]
        public bool HasValue => _value != null;

        [PublicAPI]
        public bool IsNull => _value == null;

        public Observable()
        {
        }

        public Observable(TValue value)
        {
            _value = value;
        }
    }
}