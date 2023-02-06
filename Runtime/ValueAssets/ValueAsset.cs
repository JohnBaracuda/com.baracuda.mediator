using Baracuda.Utilities.Callbacks;
using Baracuda.Utilities.Inspector;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Mediator
{
    /// <summary>
    /// Scriptable object holding a value that can be accessed and set.
    /// Object can be subscribed to receive a callback when the arg is changed.
    /// The arg of this object will reset to its inspector arg after runtime.
    /// </summary>
    public abstract class ValueAsset<TValue> : ScriptableObject, IValueAsset<TValue>, IOnExitEdit, IOnExitPlay
    {
        [SerializeField] private TValue value;

        [Readonly]
        [SerializeField] private TValue cached;

        public IReceiver<TValue> Changed => _changed;

        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(value, this.value))
                {
                    return;
                }
                this.value = value;
                _changed.Raise(value);
            }
        }

        private readonly Broadcast<TValue> _changed = new();

        #region Editor

#if UNITY_EDITOR

        private void OnEnable()
        {
            EngineCallbacks.AddCallbacks(this);
        }

        public void OnExitEditMode()
        {
            cached = Value;
        }

        public void OnExitPlayMode()
        {
            Value = cached;
        }

        [Button]
        public void ResetValue()
        {
            Value = cached;
        }
#endif

        #endregion
    }


    public abstract class ValueAssetRW<TValue> : ScriptableObject
    {
        private TValue _value;

        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(value, _value))
                {
                    return;
                }
                _value = value;
                _changed.Raise(value);
            }
        }

        public IReceiver<TValue> Changed => _changed;
        private Broadcast<TValue> _changed;

    }

    public abstract class ValueAssetRO<TValue> : ScriptableObject
    {
        [SerializeField] private TValue value;
        public TValue Value => value;
    }
}