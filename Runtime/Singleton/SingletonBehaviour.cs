using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Bedrock.Singleton
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        [HideIf(nameof(DisableDontDestroyOnLoadOption))]
        [SerializeField] private bool dontDestroyOnLoad;

        /// <summary>
        ///     The current singleton instance.
        /// </summary>
        [PublicAPI]
        public static T Singleton { get; private set; }

        /// <summary>
        ///     True if a singleton instance exists.
        /// </summary>
        [PublicAPI]
        public static bool SingletonInitialized { get; private set; }

        /// <summary>
        ///     Optional setting to disable dont destroy on load per type
        /// </summary>
        protected virtual bool DisableDontDestroyOnLoadOption { get; } = false;

        protected virtual void Awake()
        {
            if (Singleton != null)
            {
                Debug.LogWarning("Singleton", $"More that one instance of {typeof(T).Name} found!");
                return;
            }

            Singleton = (T) this;
            SingletonInitialized = true;

            if (dontDestroyOnLoad && !DisableDontDestroyOnLoadOption)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Singleton != this)
            {
                return;
            }

            Singleton = null;
            SingletonInitialized = false;
        }
    }
}