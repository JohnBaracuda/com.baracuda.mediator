using UnityEngine;

namespace Baracuda.Mediator.Services
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
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