using Baracuda.Utilities.Inspector;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator
{
    /// <summary>
    /// Scriptable object holding a value that can be accessed and set.
    /// Object can be subscribed to receive a callback when the arg is changed.
    /// The arg of this object will reset to its inspector arg after runtime.
    /// </summary>
    public abstract class ValueAsset<TValue> : ScriptableObject, IValueAsset<TValue>
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

        private readonly Broadcast<TValue> _changed;

        #region Editor

#if UNITY_EDITOR

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