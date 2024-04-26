using UnityEngine;

namespace Baracuda.Mediator.Services
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public class ServiceLocatorGlobal : ServiceLocator
    {
        [SerializeField] private bool dontDestroyOnLoad = true;

        private bool _hasBeenBootstrapped;

        private void Awake()
        {
            BootstrapOnDemand();
        }

        public void BootstrapOnDemand()
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
            Global = this;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}