using System.Runtime.CompilerServices;
using Baracuda.Mediator.Events;
using Baracuda.Mediator.ValueObjects.Abstraction;
using Baracuda.Utilities.Inspector;
using UnityEngine;

namespace Baracuda.Mediator.ValueObjects
{
    /// <summary>
    /// Scriptable object holding a value that can be accessed and set.
    /// Object can be subscribed to receive a callback when the value is changed.
    /// The value of this object will reset to its inspector value after runtime.
    /// </summary>
    [HideMonoScript]
    public abstract class ValueObjectAssetT<TValue> : EventChannel<TValue>, IValueObject<TValue>, __IValueObject
    {
        [Foldout(FoldoutName.HumanizedObjectName)]
        [SerializeField] private TValue value;

        [Readonly]
        [SerializeField] private TValue cached;

        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;
            set
            {
                this.value = value;
                Raise(value);
            }
        }

        /*
         * Operator
         */

        public static implicit operator TValue(ValueObjectAssetT<TValue> valueObjectAsset)
        {
            return valueObjectAsset.Value;
        }

        /*
         * Unity Editor Utilities
         */

#if UNITY_EDITOR
        private void OnValidate()
        {
            Raise(Value);
        }

        private void OnEnable()
        {
            UnityEditor.EditorApplication.playModeStateChanged += PlayStateChanged;
        }

        private void OnDisable()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= PlayStateChanged;
        }

        private void PlayStateChanged(UnityEditor.PlayModeStateChange change)
        {
            switch (change)
            {
                case UnityEditor.PlayModeStateChange.ExitingEditMode:
                    cached = Value;
                    return;
                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    Value = cached;
                    return;
            }
        }

        /*
         * Raw Value
         */

        public object GetRawValue()
        {
            return Value as object;
        }

        [Button]
        public void ResetValue()
        {
            Value = cached;
        }
#endif

    }
}