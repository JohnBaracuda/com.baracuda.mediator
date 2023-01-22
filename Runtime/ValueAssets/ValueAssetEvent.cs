using Baracuda.Utilities.Inspector;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator
{
    /// <summary>
    /// Scriptable object holding a arg that can be accessed and set.
    /// Object can be subscribed to receive a callback when the arg is changed.
    /// The arg of this object will reset to its inspector arg after runtime.
    /// </summary>
    public abstract class ValueAssetEvent<TValue> : IValueAsset<TValue>, IBroadcast<TValue>
    {
        [SerializeField] private TValue value;
        [Readonly] [SerializeField] private TValue cached;

        private readonly IBroadcast<TValue> _event = new Broadcast<TValue>();

        public sealed override TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
            set
            {
                this.value = value;
                Raise(value);
            }
        }

        /// <inheritdoc />
        public void Add(Action<TValue> listener)
        {
            _event.Add(listener);
        }

        /// <inheritdoc />
        public bool AddUnique(Action<TValue> listener)
        {
            return _event.AddUnique(listener);
        }

        /// <inheritdoc />
        public bool Remove(Action<TValue> listener)
        {
            return _event.Remove(listener);
        }

        /// <inheritdoc />
        public void Raise(TValue arg)
        {
            _event.Raise(arg);
        }

        /// <inheritdoc />
        public bool Contains(Action<TValue> listener)
        {
            return _event.Contains(listener);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _event.Clear();
        }

        /// <inheritdoc />
        public void ClearInvalid()
        {
            _event.ClearInvalid();
        }

        public static implicit operator TValue(ValueAssetEvent<TValue> valueAssetEvent)
        {
            return valueAssetEvent.Value;
        }

        #region Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            Raise(Value);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += PlayStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayStateChanged;
        }

        private void PlayStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.ExitingEditMode:
                    cached = Value;
                    return;
                case PlayModeStateChange.ExitingPlayMode:
                    Value = cached;
                    _event.Clear();
                    return;
            }
        }

        /*
         * Raw Value
         */

        [Button]
        public void ResetValue()
        {
            Value = cached;
        }
#endif

        #endregion
    }
}