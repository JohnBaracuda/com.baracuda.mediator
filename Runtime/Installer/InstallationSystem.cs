using Baracuda.Mediator.Callbacks;
using Baracuda.Mediator.Singleton;
using Baracuda.Utilities;
using Baracuda.Utilities.Types;
using UnityEngine;
using UnityEngine.Serialization;

namespace Baracuda.Mediator.Installer
{
    public class InstallationSystem : SingletonAsset<InstallationSystem>
    {
        [SerializeField] private bool autoInstall;
        [FormerlySerializedAs("systems")]
        [SerializeField] private Optional<GameObject>[] prefabs;

        private static bool installed;

        [CallbackOnInitialization]
        private void Install()
        {
            if (installed || autoInstall is false)
            {
                return;
            }
            installed = true;
            foreach (var system in prefabs)
            {
                if (system.TryGetValue(out var prefab))
                {
                    var instance = Instantiate(prefab);
                    instance.DontDestroyOnLoad();
                    instance.name = $"[{prefab.name}]";
                }
            }
        }

        [CallbackOnApplicationQuit]
        private void OnQuit()
        {
            installed = false;
        }
    }
}