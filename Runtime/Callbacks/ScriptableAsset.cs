﻿using Baracuda.Mediator.Injection;
using Baracuda.Mediator.Utility;
using Baracuda.Tools;
using Baracuda.Utilities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Baracuda.Mediator.Callbacks
{
    /// <summary>
    ///     Abstract base class for <see cref="ScriptableObject" />s that can receive <see cref="Gameloop" /> callbacks.
    ///     Use the <see cref="CallbackMethodAttribute" /> to receive custom callbacks on a target method.
    /// </summary>
    public abstract class ScriptableAsset : ScriptableObject
    {
        [Flags]
        private enum Options
        {
            None = 0,

            /// <summary>
            ///     When enabled, the asset will receive custom runtime and editor callbacks.
            /// </summary>
            [Description("When enabled, the asset will receive custom runtime and editor callbacks.")]
            ReceiveCallbacks = 1,

            /// <summary>
            ///     When enabled, a developer annotation field is displayed.
            /// </summary>
            [Description("When enabled, a developer annotation field is displayed.")]
            Annotation = 2,

            /// <summary>
            ///     When enabled, changes to this asset during runtime are reset when entering edit mode.
            /// </summary>
            [Description("When enabled, changes to this asset during runtime are reset when entering edit mode.")]
            ResetRuntimeChanges = 4,

            /// <summary>
            ///     When enabled, dependencies are not automatically injected after the first scene was loaded.
            /// </summary>
            DontInjectDependencies = 8
        }

        [PropertySpace(0, 8)]
        [Tooltip(AssetOptionsTooltip)]
        [PropertyOrder(-10000)]
        [HideInInlineEditors]
        [SerializeField] private Options assetOptions = Options.ReceiveCallbacks;

#pragma warning disable
        [Line(SpaceBefore = 0)]
        [TextArea(0, 6)]
        [UsedImplicitly]
        [ShowIf(nameof(ShowAnnotation))]
        [FormerlySerializedAs("description")]
        [PropertyOrder(-9999)]
        [PropertySpace(0, 8)]
        [SerializeField] private string annotation;
#pragma warning restore

        private const string AssetOptionsTooltip =
            "Receive Callbacks: When enabled, the asset will receive custom runtime and editor callbacks." +
            "Annotation: When enabled, a developer annotation field is displayed." +
            "ResetRuntimeChanges: When enabled, changes to this asset during runtime are reset when entering edit mode.";

        private bool ShowAnnotation => assetOptions.HasFlagFast(Options.Annotation);

#if UNITY_EDITOR
        [ShowInInlineEditors]
        [ShowInInspector]
        [PropertyOrder(-1)]
        [PropertySpace(0, 8)]
        private Object Script
        {
            get
            {
                _serializedObject ??= new UnityEditor.SerializedObject(this);
                return _serializedObject.FindProperty("m_Script").objectReferenceValue;
            }
        }

        private UnityEditor.SerializedObject _serializedObject;
#endif

        [Conditional("UNITY_EDITOR")]
        public void Repaint()
        {
#if UNITY_EDITOR
            if (Gameloop.IsQuitting)
            {
                return;
            }

            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        protected virtual void OnEnable()
        {
            if (assetOptions.HasFlagFast(Options.ReceiveCallbacks))
            {
                Gameloop.Register(this);
            }
        }

        /// <summary>
        ///     Reset the asset to its default values.
        /// </summary>
        public void ResetAsset()
        {
            ScriptableAssetUtility.ResetAsset(this);
        }

        [CallbackMethod("Dependencies")]
        protected void HandleDependencies()
        {
            if (assetOptions.HasFlagFast(Options.DontInjectDependencies) is false)
            {
                Inject.Dependencies(this);
            }
        }


        #region Editor

#if UNITY_EDITOR

        [NonSerialized] private string _json;

        [CallbackOnEnterPlayMode]
        private void OnEnterPlayMode()
        {
            if (assetOptions.HasFlagFast(Options.ResetRuntimeChanges))
            {
                _json = ScriptableAssetUtility.GetAssetJSon(this);
            }
        }

        [CallbackOnExitPlayMode]
        private void OnExitPlayMode()
        {
            if (assetOptions.HasFlagFast(Options.ResetRuntimeChanges))
            {
                ScriptableAssetUtility.SetAssetJSon(this, _json);
            }
        }

#endif

        #endregion
    }
}