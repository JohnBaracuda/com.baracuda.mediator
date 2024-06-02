using Baracuda.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace Baracuda.Bedrock.Services
{
    public static class ServiceContainerExtensions
    {
        [PublicAPI]
        public static ServiceContainer AddSingletonNew<T>(this ServiceContainer container) where T : class, new()
        {
            var instance = new T();
            container.AddSingleton(instance);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonMonoBehaviour<T>(this ServiceContainer container)
            where T : MonoBehaviour
        {
            var instance = new GameObject($"[{typeof(T).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<T>();
            container.AddSingleton(service);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonMonoBehaviour<TInterface, TImplementation>(
            this ServiceContainer container)
            where TImplementation : MonoBehaviour, TInterface
        {
            var instance = new GameObject($"[{typeof(TInterface).Name}] [{typeof(TImplementation).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<TImplementation>();
            container.AddSingleton<TInterface>(service);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonPrefabInstance<T>(this ServiceContainer container, T prefab)
            where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab);
            instance.name = $"[{typeof(T).Name}]";
            instance.DontDestroyOnLoad();
            container.AddSingleton(instance);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonPrefabInstance<T>(this ServiceContainer container, GameObject prefab)
            where T : MonoBehaviour
        {
            var gameObject = Object.Instantiate(prefab);
            var instance = gameObject.GetComponent<T>();
            instance.DontDestroyOnLoad();
            container.AddSingleton(instance);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonPrefabInstanceLazy<T>(this ServiceContainer container, T prefab)
            where T : MonoBehaviour
        {
            container.AddLazy(() =>
            {
                var instance = Object.Instantiate(prefab);
                instance.name = $"[{typeof(T).Name}]";
                instance.DontDestroyOnLoad();
                return instance;
            });
            return container;
        }
    }
}