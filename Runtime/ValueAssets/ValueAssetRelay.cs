using Baracuda.Mediator.Relays;
using Baracuda.Utilities.Inspector;
using JetBrains.Annotations;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Mediator.ValueAssets
{
    /// <summary>
    /// Scriptable object holding a arg that can be accessed and set.
    /// Object can be subscribed to receive a callback when the arg is changed.
    /// The arg of this object will reset to its inspector arg after runtime.
    /// </summary>
    [HideMonoScript]
    public abstract class ValueAssetRelay<TValue> : IValueAsset<TValue>, IRelay<TValue>
    {
        [UsedImplicitly]
        [SerializeField] private string description;

        [Foldout(FoldoutName.HumanizedObjectName)]
        [SerializeField] private TValue value;

        [Readonly]
        [SerializeField] private TValue cached;

        private readonly IRelayBroadcast<TValue> _relay = new RelayBroadcast<TValue>();

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

        public void Add(Action<TValue> listener)
        {
            _relay.Add(listener);
        }

        public bool AddUnique(Action<TValue> listener)
        {
            return _relay.AddUnique(listener);
        }

        public bool Remove(Action<TValue> listener)
        {
            return _relay.Remove(listener);
        }

        public void Raise(in TValue arg)
        {
            _relay.Raise(arg);
        }

        public bool Contains(Action<TValue> listener)
        {
            return _relay.Contains(listener);
        }

        public void Clear()
        {
            _relay.Clear();
        }

        public static implicit operator TValue(ValueAssetRelay<TValue> valueAssetRelay)
        {
            return valueAssetRelay.Value;
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
                    _relay.Clear();
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