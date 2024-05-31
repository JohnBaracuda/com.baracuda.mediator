using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Baracuda.Bedrock.Scenes
{
    /// <summary>
    ///     Trigger to perform async level loading
    /// </summary>
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    public class LevelGate : MonoBehaviour
    {
        [FormerlySerializedAs("addressableLevelAsset")]
        [FormerlySerializedAs("sceneLayoutAsset")]
        [FormerlySerializedAs("multiSceneAsset")]
        [RequiredIn(PrefabKind.InstanceInScene)]
        [SerializeField] private LevelAsset levelAsset;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ISceneLoadTrigger>(out var trigger) && trigger.CanTriggerSceneLoad())
            {
                PreformSceneTransitionAsync().Forget();
            }
        }

        private async UniTask PreformSceneTransitionAsync()
        {
            await levelAsset.LoadAsync();
        }
    }
}