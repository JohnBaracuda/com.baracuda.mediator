using System;
using UnityEngine.AddressableAssets;

namespace Baracuda.Mediator.Scenes
{
    [Serializable]
    public class AssetReferenceScene
#if UNITY_EDITOR
        : AssetReferenceT<UnityEditor.SceneAsset>
#else
    : AssetReference
#endif
    {
        public AssetReferenceScene(string guid) : base(guid)
        {
        }
    }
}