using Baracuda.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace Baracuda.Bedrock.Services
{
    public static class ServiceContainerExtensions
    {
        [PublicAPI]
        public static ServiceContainer AddSingleton<TInterface, T>(this ServiceContainer container) where T : TInterface, new()
        {
            var instance = new T();
            container.Add<TInterface>(instance);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingleton<T>(this ServiceContainer container) where T : class, new()
        {
            var instance = new T();
            container.Add(instance);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonBehaviour<T>(this ServiceContainer container) where T : MonoBehaviour
        {
            var instance = new GameObject($"[{typeof(T).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<T>();
            container.Add(service);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonBehaviour<TInterface, T>(this ServiceContainer container) where T : MonoBehaviour, TInterface
        {
            var instance = new GameObject($"[{typeof(TInterface).Name}] [{typeof(T).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<T>();
            container.Add<TInterface>(service);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonPrefab<T>(this ServiceContainer container, T prefab) where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab);
            instance.name = $"[{typeof(T).Name}]";
            instance.DontDestroyOnLoad();
            container.Add(instance);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonPrefab<T>(this ServiceContainer container, GameObject prefab) where T : MonoBehaviour
        {
            var gameObject = Object.Instantiate(prefab);
            var instance = gameObject.GetComponent<T>();
            instance.DontDestroyOnLoad();
            container.Add(instance);
            return container;
        }

        [PublicAPI]
        public static ServiceContainer AddSingletonPrefabLazy<T>(this ServiceContainer container, T prefab) where T : MonoBehaviour
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