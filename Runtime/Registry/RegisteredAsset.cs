﻿using Baracuda.Bedrock.Callbacks;
using Baracuda.Utilities.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Bedrock.Registry
{
    /// <summary>
    ///     Registered assets are always included in a build and loaded during startup.
    ///     They can also be resolved during runtime, using their GUID.
    /// </summary>
    public abstract class RegisteredAsset : ScriptableAsset, IAssetGUID
    {
        [PropertySpace]
        [PropertyOrder(-10001)]
        [SerializeField] private RuntimeGUID guid;
        public RuntimeGUID GUID => guid;

#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
            RuntimeGUID.Create(this, ref guid);
            AssetRegistry.RegisterAsset(this);
        }

        protected virtual void OnValidate()
        {
            AssetRegistry.RegisterAsset(this);
        }
#endif
    }
}