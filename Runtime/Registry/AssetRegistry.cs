using Baracuda.Mediator.Callbacks;
using Baracuda.Tools;
using Baracuda.Utilities;
using Baracuda.Utilities.Reflection;
using Baracuda.Utilities.Types;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Mediator.Registry
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    [CreateAssetMenu]
    [AddressablesGroup("Preload", CreateLabel = true)]
    public sealed class AssetRegistry : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Inspector

        [Space]
        [SerializeField] private List<Object> singletons;
        [Line]
        [SerializeField] private Map<int, Object> registry = new();

        #endregion


        #region Properties

        public static bool IsLoaded => singleton != null;

        #endregion


        #region Singleton API

        /// <summary>
        ///     Register a singleton object. The object is then cached persistently and can be resolved with by its type.
        /// </summary>
        public static void RegisterSingleton<T>(T instance) where T : Object
        {
            RegisterSingletonInternal(instance);
        }

        /// <summary>
        ///     Get the singleton instance for T. Use <see cref="ExistsSingleton{T}" /> to check if an instance is registered.
        /// </summary>
        /// <typeparam name="T">The type of the singleton instance to resolve</typeparam>
        /// <returns>The singleton instance of T</returns>
        public static T ResolveSingleton<T>() where T : Object
        {
            return ResolveSingletonInternal<T>();
        }

        public static bool ExistsSingleton<T>()
        {
            return ExistsSingletonInternal<T>();
        }

        #endregion


        #region Asset API

        /// <summary>
        ///     Collection of every registered asset.
        /// </summary>
        public static IReadOnlyDictionary<int, Object> Registry => Singleton.registry;

        /// <summary>
        ///     Register a unique asset. Registered assets can be resolved using their <see cref="RuntimeGUID" />
        /// </summary>
        public static void RegisterAsset<T>(T asset) where T : Object, IAssetGUID
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (asset.GUID.Hash == 0)
                {
                    Debug.Log(asset);
                }
                Singleton.registry.AddOrUpdate(asset.GUID.Hash, asset);
            };
#endif
        }

        /// <summary>
        ///     Resolve an asset of type T by its GUID
        /// </summary>
        public static T ResolveAsset<T>(string guid) where T : Object
        {
            return (T) Singleton.registry[guid.GetHashCode()];
        }

        /// <summary>
        ///     Resolve an asset of type T by its GUID Hash
        /// </summary>
        public static T ResolveAsset<T>(int hash) where T : Object
        {
            return (T) Singleton.registry[hash];
        }

        /// <summary>
        ///     Resolve an asset of type T by its GUID
        /// </summary>
        public static T ResolveAsset<T>(RuntimeGUID guid) where T : Object
        {
            return (T) Singleton.registry[guid.Hash];
        }

        /// <summary>
        ///     Try Resolve an asset of type T by its GUID
        /// </summary>
        public static bool TryResolveAsset<T>(string guid, out T result) where T : Object
        {
            if (Singleton.registry.TryGetValue(guid.GetHashCode(), out var instance))
            {
                result = instance as T;
                return result != null;
            }

            result = default(T);
            return false;
        }

        /// <summary>
        ///     Try Resolve an asset of type T by its GUID
        /// </summary>
        public static bool TryResolveAsset<T>(int hash, out T result) where T : Object
        {
            if (Singleton.registry.TryGetValue(hash, out var instance))
            {
                result = instance as T;
                return result != null;
            }

            result = default(T);
            return false;
        }

        /// <summary>
        ///     Get the ID of the passed asset. Returns -1 if no asset was found
        /// </summary>
        /// <param name="asset"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetIdOfAsset<T>(T asset) where T : Object
        {
            foreach (var (key, value) in Singleton.registry)
            {
                if (value == asset)
                {
                    return key;
                }
            }

            return -1;
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RegisterSingletonInternal<T>(T instance) where T : Object
        {
#if UNITY_EDITOR

            if (IsImport())
            {
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                WaitWhile(IsImport).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return;
                    }
                    RegisterSingleton(instance);
                }, scheduler);
                return;
            }
#endif
            if (IsLoaded is false)
            {
                Debug.LogError("Singleton",
                    $"Registry is not loaded yet! Cannot register instance for {typeof(T)}",
                    instance);
                return;
            }

            for (var i = 0; i < Singleton.singletons.Count; i++)
            {
                var entry = Singleton.singletons[i];
                if (entry != null && entry is T)
                {
                    Singleton.singletons[i] = instance;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(Singleton);
#endif
                    return;
                }
            }

            Singleton.singletons.Add(instance);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(Singleton);
#endif

            return;

#if UNITY_EDITOR
            static async Task WaitWhile(Func<bool> condition)
            {
                while (condition())
                {
                    await Task.Delay(25);
                }
            }

            static bool IsImport()
            {
                return UnityEditor.EditorApplication.isCompiling || UnityEditor.EditorApplication.isUpdating;
            }
#endif
        }

        private static T ResolveSingletonInternal<T>() where T : Object
        {
            for (var i = 0; i < Singleton.singletons.Count; i++)
            {
                var element = Singleton.singletons[i];
                if (element != null && element is T instance)
                {
                    return instance;
                }
            }

            Debug.LogError("Singleton", $"No singleton instance of type {typeof(T)} registered!",
                Singleton);
            return null;
        }

        private static bool ExistsSingletonInternal<T>()
        {
            if (Singleton.singletons == null)
            {
                Debug.LogError("Singleton",
                    $"Registry is null! Attempted to access singleton for {typeof(T)}");
                return false;
            }

            for (var i = 0; i < Singleton.singletons.Count; i++)
            {
                var entry = Singleton.singletons[i];

                if (entry == null)
                {
                    continue;
                }

                if (entry.GetType() == typeof(T))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Singleton

        private static AssetRegistry singleton;

        public static AssetRegistry Singleton
        {
            get
            {
                // In the editor we load the singleton from the asset database.
#if UNITY_EDITOR
                if (singleton != null)
                {
                    return singleton;
                }

                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(AssetRegistry)}");
                for (var i = 0; i < guids.Length; i++)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                    singleton = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetRegistry>(path);
                    if (singleton != null)
                    {
                        break;
                    }
                }
                if (singleton == null)
                {
                    Debug.LogError("Singleton",
                        "Singleton Registry is null! Please create a new Singleton Registry (ScriptableObject)");
                }
#endif

                return singleton;
            }
        }

        #endregion


        #region Serialization

        private void OnEnable()
        {
            singleton = this;
        }

        public void OnAfterDeserialize()
        {
            singleton = this;
#if UNITY_EDITOR
            Validate();
#endif
        }

        public void OnBeforeSerialize()
        {
        }

        #endregion


        #region Editor

#if UNITY_EDITOR

        [Button]
        [Line(DrawTiming = DrawTiming.Before)]
        [Tooltip("Remove null objects from the registry")]
        public void Validate()
        {
            var assets = registry.ToArray();
            foreach (var (key, value) in assets)
            {
                if (value == null || value is not IAssetGUID)
                {
                    Debug.Log("Asset Registry", "Removing invalid unique asset registry entry!");
                    registry.Remove(key);
                }
            }

            for (var index = singletons.Count - 1; index >= 0; index--)
            {
                if (singletons[index] == null)
                {
                    singletons.RemoveAt(index);
                }
            }
        }

        static AssetRegistry()
        {
            Gameloop.BeforeDeleteAsset += OnBeforeDeleteAsset;
        }

        private static void OnBeforeDeleteAsset(string assetPath, Object asset)
        {
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            Singleton.registry.TryRemove(guid.GetHashCode());
            Singleton.singletons.Remove(asset);
        }

#endif

        #endregion
    }
}