using System;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.PlayerLoop
{
#if UNITY_EDITOR
    public sealed partial class Gameloop : UnityEditor.AssetModificationProcessor
    {
        #region Asset Handling

        private static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string assetPath,
            UnityEditor.RemoveAssetOptions options)
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            try
            {
                BeforeDeleteAsset?.Invoke(assetPath, asset);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            UnregisterInternal(asset);

            return UnityEditor.AssetDeleteResult.DidNotDelete;
        }

        #endregion
    }
#endif
}