using Baracuda.Mediator.Callbacks;
using Baracuda.Monitoring;
using Baracuda.Utilities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Baracuda.Mediator.Services
{
    [DisallowMultipleComponent]
    public abstract class ServiceLocator : MonoBehaviour, IServiceProvider<ServiceLocator>
    {
        #region Fields

        private static ServiceLocator global;
        private static readonly List<GameObject> gameObjectBuffer = new();
        protected static Dictionary<Scene, ServiceLocator> SceneLocators { get; } = new();

        private readonly ServiceContainer _container = new();

        private const string GlobalServiceLocatorName = "ServiceLocator [Global]";
        private const string SceneServiceLocatorName = "ServiceLocator [Scene]";

        #endregion


        #region Service Locator Access

        [PublicAPI]
        public static ServiceLocator Global
        {
            get
            {
                if (global != null)
                {
                    return global;
                }

                if (FindFirstObjectByType<ServiceLocatorGlobal>().IsNotNull(out var locator))
                {
                    locator.BootstrapOnDemand();
                    return global;
                }

#if UNITY_EDITOR
                if (Gameloop.IsQuitting)
                {
                    return null;
                }
#endif

                var container = new GameObject(GlobalServiceLocatorName);
                container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();
                return global;
            }
            protected set => global = value;
        }

        [PublicAPI]
        public static ServiceLocator Scene
        {
            get
            {
                var scene = SceneManager.GetActiveScene();

                if (SceneLocators.TryGetValue(scene, out var container))
                {
                    return container;
                }

                gameObjectBuffer.Clear();
                scene.GetRootGameObjects(gameObjectBuffer);

                foreach (var gameObject in gameObjectBuffer.Where(gameObject =>
                             gameObject.GetComponent<ServiceLocatorScene>() != null))
                {
                    if (gameObject.TryGetComponent(out ServiceLocatorScene bootstrapper))
                    {
                        bootstrapper.BootstrapOnDemand();
                        return bootstrapper;
                    }
                }

                return Global;
            }
        }

        [PublicAPI]
        public static ServiceLocator ForSceneOf(MonoBehaviour monoBehaviour)
        {
            var scene = monoBehaviour.gameObject.scene;

            if (SceneLocators.TryGetValue(scene, out var container) && container != monoBehaviour)
            {
                return container;
            }

            gameObjectBuffer.Clear();
            scene.GetRootGameObjects(gameObjectBuffer);

            foreach (var gameObject in gameObjectBuffer.Where(gameObject =>
                         gameObject.GetComponent<ServiceLocatorScene>() != null))
            {
                if (gameObject.TryGetComponent(out ServiceLocatorScene bootstrapper) &&
                    bootstrapper != monoBehaviour)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper;
                }
            }

            return Global;
        }

        #endregion


        #region Service Provider

        [ShowInInspector]
        [PublicAPI]
        public IEnumerable<object> Services => _container.Services;

        [PublicAPI]
        public T Get<T>() where T : class
        {
            return _container.Get<T>();
        }

        [PublicAPI]
        public object Get(Type type)
        {
            return _container.Get(type);
        }

        [PublicAPI]
        public bool TryGet<T>(out T service) where T : class
        {
            return _container.TryGet(out service);
        }

        [PublicAPI]
        public bool TryGet(Type type, out object value)
        {
            return _container.TryGet(type, out value);
        }

        [PublicAPI]
        public ServiceLocator Register(Type type, object service)
        {
            _container.Register(type, service);
            return this;
        }

        [PublicAPI]
        public ServiceLocator Register<T>(T service)
        {
            _container.Register(service);
            return this;
        }

        [PublicAPI]
        public ServiceLocator Remove<T>(T service)
        {
            _container.Remove(service);
            return this;
        }

        [PublicAPI]
        public ServiceLocator Remove<T>()
        {
            _container.Remove<T>();
            return this;
        }

        [PublicAPI]
        public void AddRegistrationCallback<T>(Action<T> callback, bool callRetroactively = true) where T : class
        {
            _container.AddRegistrationCallback(callback, callRetroactively);
        }

        [PublicAPI]
        public void RemoveRegistrationCallback<T>(Action<T> callback)
        {
            _container.RemoveRegistrationCallback(callback);
        }

        #endregion


        #region Setup

        private void OnEnable()
        {
            Monitor.StartMonitoring(_container);
        }

        private void OnDisable()
        {
            Monitor.StopMonitoring(_container);
        }

        private void OnDestroy()
        {
            if (this == global)
            {
                global = null;
            }
            else if (SceneLocators.ContainsValue(this))
            {
                SceneLocators.Remove(gameObject.scene);
            }
        }

        // https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            global = null;
            SceneLocators.Clear();
            gameObjectBuffer.Clear();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/ServiceLocator/Add Global")]
        private static void AddGlobal()
        {
            var go = new GameObject(GlobalServiceLocatorName, typeof(ServiceLocatorGlobal));
        }

        [UnityEditor.MenuItem("GameObject/ServiceLocator/Add Scene")]
        private static void AddScene()
        {
            var go = new GameObject(SceneServiceLocatorName, typeof(ServiceLocatorScene));
        }
#endif

        #endregion
    }
}