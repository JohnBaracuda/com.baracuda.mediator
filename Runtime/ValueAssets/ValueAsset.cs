using Baracuda.Utilities.Inspector;
using JetBrains.Annotations;
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
    public abstract class ValueAsset<TValue> : IValueAsset<TValue>
    {
        [SerializeField] private TValue value;

        [Readonly]
        [SerializeField] private TValue cached;

        public sealed override TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
            set => this.value = value;
        }

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