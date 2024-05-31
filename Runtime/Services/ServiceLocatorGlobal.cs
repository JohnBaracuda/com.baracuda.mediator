using UnityEngine;

namespace Baracuda.Bedrock.Services
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
            GlobalLocator = this;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}