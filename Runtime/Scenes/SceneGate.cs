using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Bedrock.Scenes
{
    /// <summary>
    ///     Trigger to perform async scene loading / unloading
    /// </summary>
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    public class SceneGate : MonoBehaviour
    {
        [Header("Loading")]
        [SerializeField] private bool loadScenes;
        [ShowIf(nameof(loadScenes))]
        [SerializeField] private SceneAsset[] loadSceneAssets;
        [Header("Unloading")]
        [SerializeField] private bool unloadScenes;
        [ShowIf(nameof(unloadScenes))]
        [SerializeField] private SceneAsset[] unloadSceneAssets;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ISceneLoadTrigger>(out var trigger) && trigger.CanTriggerSceneLoad())
            {
                PreformSceneTransitionAsync().Forget();
            }
        }

        private async UniTask PreformSceneTransitionAsync()
        {
            await UnloadAsyncInternal();
            await LoadAsyncInternal();
        }

        private async UniTask LoadAsyncInternal()
        {
            foreach (var sceneAsset in loadSceneAssets)
            {
                await sceneAsset.LoadAsync();
            }
        }

        private async UniTask UnloadAsyncInternal()
        {
            foreach (var sceneAsset in unloadSceneAssets)
            {
                await sceneAsset.UnLoadAsync();
            }
        }
    }
}