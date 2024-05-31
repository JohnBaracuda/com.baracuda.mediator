using UnityEngine;

namespace Baracuda.Bedrock.Services
{
    [AddComponentMenu("ServiceLocator/ServiceLocator ForActiveScene")]
    public class ServiceLocatorScene : ServiceLocator
    {
        private bool _hasBeenBootstrapped;

        private void Awake()
        {
            BootstrapOnDemand();
        }

        internal void BootstrapOnDemand()
        {
            if (_hasBeenBootstrapped)
            {
                return;
            }
            _hasBeenBootstrapped = true;
            Bootstrap();
        }

        private void Bootstrap()
        {
            var scene = gameObject.scene;

            if (SceneLocators.ContainsKey(scene))
            {
                Debug.LogError(
                    "ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene",
                    this);
                return;
            }

            SceneLocators.Add(scene, this);
        }
    }
}