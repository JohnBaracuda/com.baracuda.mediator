using Baracuda.Utilities.Callbacks;
using Baracuda.Utilities.Inspector;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Mediator
{
    /// <summary>
    /// Scriptable object holding a value that can be accessed and set during runtime.
    /// </summary>
    public abstract class ValueAsset<TValue> : ScriptableObject, IOnExitEdit, IOnEnterEdit
    {
        [SerializeField] private TValue value;

        [Readonly]
        [SerializeField] private TValue cached;

        private readonly Broadcast<TValue> _changedEvent = new();

        /// <summary>
        /// Event, called every time the value changed.
        /// </summary>
        public IReceiver<TValue> Changed => _changedEvent;

        /// <summary>
        /// Get or set the underlying value.
        /// </summary>
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
                _changedEvent.Raise(value);
            }
        }

        public static implicit operator TValue(ValueAsset<TValue> valueAsset)
        {
            return valueAsset.Value;
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            EngineCallbacks.AddExitEditModeListener(this);
            EngineCallbacks.AddEnterEditModeListener(this);
#else
            // Set the cached value to a default value (null for reference types) to release potential
            // references, keeping them in memory indefinitely.
            cached = default;
#endif
        }

        public void OnExitEditMode()
        {
#if UNITY_EDITOR
            UpdateCached();
#endif
        }

        public void OnEnterEditMode()
        {
#if UNITY_EDITOR
            ResetValue();
#endif
        }

#if UNITY_EDITOR
        [SpaceBefore]
        [Button(ButtonType.Center)]
        [Tooltip("Reset the current value to the cached value")]
        private void ResetValue()
        {
            Value = cached;
        }

        private void UpdateCached()
        {
            cached = Value;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                cached = Value;
            }
            else
            {
                _changedEvent.Raise(value);
            }
        }
#endif
    }
}