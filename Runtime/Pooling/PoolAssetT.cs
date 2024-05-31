#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using Baracuda.Bedrock.Callbacks;
using Baracuda.Tools;
using Baracuda.Utilities.Reflection;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.Pooling
{
    public abstract partial class PoolAsset<T> : PoolAsset,
        IDisposable,
        IObjectPool<T>
        where T : Object
    {
        #region Settings

#if UNITY_ADDRESSABLES
        [HideIf(nameof(useAddressables))]
#endif
        [SerializeField] private T prefab;

#if UNITY_ADDRESSABLES
        [SerializeField] private bool useAddressables;
        [ShowIf(nameof(useAddressables))]
        [SerializeField] private AssetReferenceT<T> prefabReference;
#endif

        [Header("Initialization")]
        [Tooltip("When enabled, the pool is created at the start of the game")]
        [SerializeField] private bool warmupOnBeginPlay;
        [SerializeField] private int initialPoolSize = 10;

        #endregion


        #region Properties

        public T Prefab => prefab;

        [ReadOnly]
        [ShowInInspector]
        [Foldout("Debug")]
        public PoolState State { get; private set; }

        public int CountAll { get; private set; }

        public int CountInactive => _pool.Count;

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        protected Transform Parent => _parent;

        #endregion


        #region Fields

        [ReadOnly]
        [ShowInInspector]
        [Foldout("Debug")]
        private readonly List<T> _pool = new();

        [ReadOnly]
        [ShowInInspector]
        [Foldout("Debug")]
        private readonly List<T> _instances = new();

        private static readonly Action<T> onGetInstanceFromPoolCallback;
        private static readonly Action<T> onReleaseInstanceToPoolCallback;

        [NonSerialized] private T _prefabData;
        [NonSerialized] private Transform _parent;

        #endregion


        #region Public

        public T Get()
        {
            return GetInternal();
        }

        public void Release(T instnace)
        {
            ReleaseInternal(instnace);
        }

        public sealed override void Load()
        {
            LoadInternal();
        }

        public void Dispose()
        {
            UnloadInternal();
        }

        public sealed override void Unload()
        {
            UnloadInternal();
        }

        public void Clear()
        {
            UnloadInternal();
        }

        #endregion


        #region Protected

        /// <summary>
        ///     Called by the pool when a new instance needs to be created.
        /// </summary>
        /// <returns>A new instance for the pool</returns>
        protected virtual T CreateInstance()
        {
            return Instantiate(_prefabData, _parent);
        }

        /// <summary>
        ///     Called by the pool when a new instance needs to be created.
        /// </summary>
        protected virtual void OnGetInstance(T instance)
        {
        }

        /// <summary>
        ///     Called by the pool when an instance is released back to the pool.
        /// </summary>
        protected virtual void OnReleaseInstance(T instance)
        {
        }

        /// <summary>
        ///     Called by the pool when it is disposed.
        /// </summary>
        protected virtual void OnDestroyInstance(T instance)
        {
            Destroy(instance);
        }

        #endregion


        #region Editor

        [Button]
        private void SelectRuntimeObject()
        {
#if UNITY_EDITOR
            if (_parent != null)
            {
                UnityEditor.Selection.activeObject = _parent;
            }
#endif
        }

        #endregion


        #region Static Ctor

        static PoolAsset()
        {
            if (typeof(T).HasInterface(typeof(IPoolObject)) is false)
            {
                return;
            }

            onGetInstanceFromPoolCallback = instance => ((IPoolObject) instance).OnGetFromPool();
            onReleaseInstanceToPoolCallback = instance => ((IPoolObject) instance).OnReleaseToPool();
        }

        #endregion


        #region Runtime Callbacks

        [CallbackOnAfterFirstSceneLoad]
        private void OnAfterFirstSceneLoad()
        {
            if (warmupOnBeginPlay)
            {
                Load();
            }
        }

        [CallbackOnApplicationQuit]
        private void OnQuit()
        {
            Dispose();
        }

        #endregion


        #region Warup And Refresh

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadInternal()
        {
            if (State != PoolState.Unloaded)
            {
                return;
            }
            State = PoolState.Loading;

            var buffer = ListPool<T>.Get();
            _parent ??= PoolTransform.Create(this);

#if UNITY_ADDRESSABLES
            if (useAddressables)
            {
                var operationHandle = prefabReference.LoadAssetAsync<T>();
                operationHandle.Completed += handle =>
                {
                    _prefabData = handle.Result;

                    for (var i = 0; i < initialPoolSize; i++)
                    {
                        buffer.Add(GetInternal());
                    }

                    foreach (var element in buffer)
                    {
                        ReleaseInternal(element);
                    }

                    ListPool<T>.Release(buffer);
                    State = PoolState.Loaded;
                };
                return;
            }
#endif

            _prefabData = prefab;

            for (var i = 0; i < initialPoolSize; i++)
            {
                buffer.Add(GetInternal());
            }

            foreach (var element in buffer)
            {
                ReleaseInternal(element);
            }

            ListPool<T>.Release(buffer);
            State = PoolState.Loaded;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnloadInternal()
        {
            AssertIsPlaying();

            foreach (var instance in _pool)
            {
                OnDestroyInstance(instance);
            }

            _pool.Clear();
            CountAll = 0;
            if (_parent != null)
            {
                Destroy(_parent.gameObject);
            }
            _parent = null;
            _prefabData = null;
#if UNITY_ADDRESSABLES
            if (useAddressables && prefabReference.IsValid())
            {
                prefabReference.ReleaseAsset();
            }
#endif
            State = PoolState.Unloaded;
        }

        #endregion


        #region Get And Release

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetInternal()
        {
            AssertIsPlaying();

            if (State == PoolState.Unloaded)
            {
                LoadInternal();
            }

            T instance;
            var isPoolEmpty = _pool.Count == 0;
            if (isPoolEmpty)
            {
                instance = CreateInstance();
                ++CountAll;
            }
            else
            {
                var index = _pool.Count - 1;
                instance = _pool[index];
                _pool.RemoveAt(index);
            }

            OnGetInstance(instance);
            onGetInstanceFromPoolCallback?.Invoke(instance);
            _instances.Add(instance);
            UpdateInspector();
            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReleaseInternal(T instance)
        {
            AssertIsPlaying();
            if (Gameloop.IsQuitting)
            {
                return;
            }
            if (instance == null)
            {
                Debug.LogError("Pooling", "Released Instance was null!");
                return;
            }

            if (_pool.Contains(instance))
            {
                Debug.Log("Pooling", $"Released Instance [{instance}] was already released to the pool!", instance);
                return;
            }

            OnReleaseInstance(instance);
            onReleaseInstanceToPoolCallback?.Invoke(instance);
            _instances.Remove(instance);

            _pool.Add(instance);

            UpdateInspector();
        }

        #endregion


        #region Obsolete & Conditional

        /// <summary>
        ///     This method is Obsolete and will throw an InvalidOperationException since we cannot create a new
        ///     <see cref="PooledObject&lt;T&gt;" /> since its constructor is internal.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [Obsolete("This method is Obsolete and will throw an InvalidOperationException")]
        public PooledObject<T> Get(out T instance)
        {
            throw new InvalidOperationException("Invalid method call!");
        }

        [Conditional("UNITY_EDITOR")]
        private static void AssertIsPlaying()
        {
            Assert.IsTrue(Application.isPlaying, "Application Is Not Playing!");
        }

        [Conditional("UNITY_EDITOR")]
        private void UpdateInspector()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        #endregion
    }
}