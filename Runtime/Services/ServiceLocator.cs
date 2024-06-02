﻿using Baracuda.Bedrock.PlayerLoop;
using Baracuda.Utilities;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Baracuda.Bedrock.Services
{
    [DisallowMultipleComponent]
    public abstract class ServiceLocator : MonoBehaviour
    {
        #region Fields

        protected static ServiceLocator GlobalLocator { get; set; }
        protected static Dictionary<Scene, ServiceLocator> SceneLocators { get; } = new();
        private static Dictionary<string, ServiceLocator> NamedLocators { get; } = new();
        private static Dictionary<Type, ServiceLocator> TypedLocators { get; } = new();

        private readonly ServiceContainer _container = new();

        private const string GlobalServiceLocatorName = "ServiceLocator [Global]";
        private const string SceneServiceLocatorName = "ServiceLocator [ForActiveScene]";

        #endregion


        #region Service Locator Access

        [PublicAPI]
        public static ServiceContainer Global => GetGlobalContainerInternal();

        [PublicAPI]
        public static ServiceContainer ForActiveScene(bool create = true)
        {
            return ForActiveSceneInternal(create);
        }

        [PublicAPI]
        [CanBeNull]
        public static ServiceContainer ForSceneOf(MonoBehaviour monoBehaviour, bool create = false)
        {
            return ForSceneOfInternal(monoBehaviour, create);
        }

        [PublicAPI]
        [CanBeNull]
        public static ServiceContainer ForScene(Scene scene, bool create = false)
        {
            return ForSceneInternal(scene, create);
        }

        [PublicAPI]
        public static ServiceContainer ForScope(string scope)
        {
            return ForScopeInternal(scope);
        }

        [PublicAPI]
        public static ServiceContainer ForScope<T>()
        {
            return ForScopeInternal<T>();
        }

        #endregion


        #region Service Provider

        [PublicAPI]
        public static async UniTask<T> GetAsync<T>() where T : class
        {
            for (;;)
            {
                if (TryResolve<T>(out var result))
                {
                    return result;
                }
                await UniTask.NextFrame();
            }
        }

        [PublicAPI]
        public static T Get<T>() where T : class
        {
            return Global.Resolve<T>();
        }

        [PublicAPI]
        public static void Get<T>(ref T field) where T : class
        {
            field = Global.Resolve<T>();
        }

        [PublicAPI]
        public static object Get(Type type)
        {
            return Global.Resolve(type);
        }

        [PublicAPI]
        public static bool TryResolve<T>(out T service) where T : class
        {
            return Global.TryResolve(out service);
        }

        [PublicAPI]
        public static bool TryResolve(Type type, out object value)
        {
            return Global.TryResolve(type, out value);
        }

        [PublicAPI]
        public static void AddSingleton<T>(T service)
        {
            Global.AddSingleton(service);
        }

        [PublicAPI]
        public static void AddSingleton(Type type, object service)
        {
            Global.AddSingleton(type, service);
        }

        [PublicAPI]
        public static void AddTransient<T>(Func<T> serviceFunc)
        {
            Global.AddTransient(serviceFunc);
        }

        [PublicAPI]
        public static void AddTransient(Type type, Delegate serviceFunc)
        {
            Global.AddTransient(type, serviceFunc);
        }

        [PublicAPI]
        public static void AddLazy<T>(Func<T> serviceFunc)
        {
            Global.AddLazy(serviceFunc);
        }

        [PublicAPI]
        public static void AddLazy(Type type, Delegate serviceFunc)
        {
            Global.AddLazy(type, serviceFunc);
        }

        [PublicAPI]
        public static void Remove<T>(T service)
        {
            Global.Remove(service);
        }

        [PublicAPI]
        public static void Remove<T>()
        {
            Global.Remove<T>();
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ServiceContainer GetGlobalContainerInternal()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            if (GlobalLocator != null)
            {
                return GlobalLocator._container;
            }

            if (FindFirstObjectByType<ServiceLocatorGlobal>().IsNotNull(out var locator))
            {
                locator.BootstrapOnDemand();
                return GlobalLocator._container;
            }

#if UNITY_EDITOR
            if (Gameloop.IsQuitting)
            {
                Debug.LogWarning("Service Locator",
                    "Please do not access service locator while the game is quitting!");
                return null;
            }
#endif

            var container = new GameObject(GlobalServiceLocatorName);
            container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();
            return GlobalLocator._container;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ServiceContainer ForActiveSceneInternal(bool create)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif

            var scene = SceneManager.GetActiveScene();

            return ForScene(scene, create);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ServiceContainer ForSceneOfInternal(MonoBehaviour monoBehaviour, bool create)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            var scene = monoBehaviour.gameObject.scene;

            if (SceneLocators.TryGetValue(scene, out var locator))
            {
                return locator._container;
            }
            return create ? CreateSceneLocator(scene)._container : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ServiceContainer ForSceneInternal(Scene scene, bool create)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            if (SceneLocators.TryGetValue(scene, out var locator))
            {
                return locator._container;
            }
            return create ? CreateSceneLocator(scene)._container : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ServiceContainer ForScopeInternal(string scope)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            if (NamedLocators.TryGetValue(scope, out var locator))
            {
                return locator._container;
            }

            var gameObject = new GameObject($"ServiceLocator [{scope}]");
            gameObject.DontDestroyOnLoad();
            var serviceLocator = gameObject.AddComponent<ServiceLocatorScene>();
            serviceLocator.BootstrapOnDemand();
            NamedLocators.Add(scope, serviceLocator);

            return serviceLocator._container;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ServiceContainer ForScopeInternal<T>()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw new InvalidOperationException("Application must be running!");
            }
#endif
            var scope = typeof(T);

            if (TypedLocators.TryGetValue(scope, out var locator))
            {
                return locator._container;
            }

            var gameObject = new GameObject($"ServiceLocator [{scope.FullName}]");
            gameObject.DontDestroyOnLoad();
            var serviceLocator = gameObject.AddComponent<ServiceLocatorScene>();
            serviceLocator.BootstrapOnDemand();
            TypedLocators.Add(scope, serviceLocator);

            return serviceLocator._container;
        }

        #endregion


        #region Setup

        private void OnDestroy()
        {
            if (this == GlobalLocator)
            {
                GlobalLocator = null;
            }
            else if (SceneLocators.ContainsValue(this))
            {
                SceneLocators.Remove(gameObject.scene);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            GlobalLocator = null;
            SceneLocators.Clear();
            NamedLocators.Clear();
            TypedLocators.Clear();

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.isSubScene)
            {
                return;
            }
            if (ForScene(scene) is not null)
            {
                return;
            }

            CreateSceneLocator(scene);
        }

        private static ServiceLocatorScene CreateSceneLocator(Scene scene)
        {
            var gameObject = new GameObject(SceneServiceLocatorName);
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            var sceneLocator = gameObject.AddComponent<ServiceLocatorScene>();
            sceneLocator.BootstrapOnDemand();
            return sceneLocator;
        }

        #endregion
    }
}