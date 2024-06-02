using Baracuda.Bedrock.Assets;
using Baracuda.Bedrock.Events;
using Baracuda.Bedrock.PlayerLoop;
using Baracuda.Utilities;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Baracuda.Bedrock.Scenes
{
    public class SceneAsset : RegisteredAsset
    {
        #region Instance

        [SerializeField] private SceneProvider sceneProvider = SceneProvider.BuildIndex;
        [ShowIf(nameof(sceneProvider), SceneProvider.Addressable)]
        [SerializeField] [Required] private AssetReferenceScene sceneReference;
        [ShowIf(nameof(sceneProvider), SceneProvider.BuildIndex)]
        [SerializeField] [Required] private int buildIndex;

        [ShowInInspector] [ReadOnly]
        public SceneState State { get; internal set; }

#if UNITY_EDITOR
        public UnityEditor.SceneAsset EditorSceneAsset
        {
            get
            {
                switch (sceneProvider)
                {
                    case SceneProvider.BuildIndex:
                        var path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
                        return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(path);
                    case SceneProvider.Addressable:
                        return sceneReference.editorAsset;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
#endif

        #endregion


        #region Static

        /// <summary>
        ///     Set of all currently loaded scene assets.
        /// </summary>
        [ShowInInspector] [ReadOnly]
        public static HashSet<SceneAsset> LoadedScenes { get; } = new();

        public static event Action<SceneAsset> BeforeLoad
        {
            add => beforeLoad.Add(value);
            remove => beforeLoad.Remove(value);
        }

        public static event Action<SceneAsset> AfterLoad
        {
            add => afterLoad.Add(value);
            remove => afterLoad.Remove(value);
        }

        public static event Action<SceneAsset> BeforeUnload
        {
            add => beforeUnload.Add(value);
            remove => beforeUnload.Remove(value);
        }

        public static event Action<SceneAsset> AfterUnload
        {
            add => afterUnload.Add(value);
            remove => afterUnload.Remove(value);
        }

        #endregion


        #region Fields

        private static readonly Broadcast<SceneAsset> beforeLoad = new();
        private static readonly Broadcast<SceneAsset> afterLoad = new();
        private static readonly Broadcast<SceneAsset> beforeUnload = new();
        private static readonly Broadcast<SceneAsset> afterUnload = new();

        #endregion


        #region Loading Operations

        public async UniTask LoadAsync(LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            switch (loadSceneMode)
            {
                case LoadSceneMode.Single:
                    await LoadSingleAsyncInternal();
                    break;
                case LoadSceneMode.Additive:
                    await LoadAdditiveAsyncInternal();
                    break;
            }
        }

        public async UniTask UnLoadAsync()
        {
            await UnloadSceneAsyncInternal();
        }

        #endregion


        #region Misc

        [CallbackOnApplicationQuit]
        private void OnQuit()
        {
            State = SceneState.None;
#if UNITY_EDITOR
            _isLoadedInEditor = false;
#endif
        }

        #endregion


        #region Loading

        private async UniTask LoadAdditiveAsyncInternal()
        {
#if UNITY_EDITOR
            switch (sceneProvider)
            {
                case SceneProvider.BuildIndex:
                    if (SceneUtility.GetScenePathByBuildIndex(buildIndex).IsNullOrEmpty())
                    {
                        Debug.LogError("Scene Asset Build Index is not valid!", this);
                        return;
                    }
                    break;
                case SceneProvider.Addressable:
                    if (sceneReference.editorAsset == null)
                    {
                        Debug.LogError("Scene Asset Reference is not set!", this);
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#endif

            if (State is not (SceneState.None or SceneState.Unloaded))
            {
                Debug.LogWarning("Scene Asset", $"Cannot load {this} because it is {State}");
                return;
            }

            Debug.Log("Scene Asset", $"Loading {this}");

            State = SceneState.Loading;
            beforeLoad.Raise(this);

            switch (sceneProvider)
            {
                case SceneProvider.BuildIndex:
                    await SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
                    break;
                case SceneProvider.Addressable:
                    await sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            LoadedScenes.Add(this);
            State = SceneState.Loaded;
            afterLoad.Raise(this);
        }

        private async UniTask LoadSingleAsyncInternal()
        {
#if UNITY_EDITOR
            switch (sceneProvider)
            {
                case SceneProvider.BuildIndex:
                    if (SceneUtility.GetScenePathByBuildIndex(buildIndex).IsNullOrEmpty())
                    {
                        Debug.LogError($"Scene Asset Build Index {buildIndex} is not valid!", this);
                        return;
                    }
                    break;
                case SceneProvider.Addressable:
                    if (sceneReference.editorAsset == null)
                    {
                        Debug.LogError("Scene Asset Reference is not set!", this);
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#endif

            if (State is not (SceneState.None or SceneState.Unloaded))
            {
                Debug.LogWarning("Scene Asset", $"Cannot load {this} because it is {State}");
                return;
            }

            Debug.Log("Scene Asset", $"Loading {this}");

            var scenes = LoadedScenes.ToArray();
            foreach (var activeScene in scenes)
            {
                activeScene.State = SceneState.Unloading;
            }

            State = SceneState.Loading;
            beforeLoad.Raise(this);

            switch (sceneProvider)
            {
                case SceneProvider.BuildIndex:
                    await SceneManager.LoadSceneAsync(buildIndex);
                    break;
                case SceneProvider.Addressable:
                    await sceneReference.LoadSceneAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var activeScene in scenes)
            {
                if (activeScene != this)
                {
                    activeScene.State = SceneState.Unloaded;
                    LoadedScenes.Remove(activeScene);
                }
            }

            LoadedScenes.Add(this);
            State = SceneState.Loaded;
            afterLoad.Raise(this);
        }

        #endregion


        #region Unloading

        private async UniTask UnloadSceneAsyncInternal()
        {
#if UNITY_EDITOR
            if (sceneReference.editorAsset == null)
            {
                Debug.LogError("ForActiveScene Reference is not set!", this);
                return;
            }
#endif

            if (State is not SceneState.Loaded)
            {
                Debug.LogWarning("ForActiveScene", $"Cannot unload {this} because it is {State}");
                return;
            }

            beforeUnload.Raise(this);
            State = SceneState.Unloading;

#if UNITY_EDITOR
            if (_isLoadedInEditor)
            {
                await UnloadEditorScene(this);
                State = SceneState.Unloaded;
                LoadedScenes.Remove(this);
                afterUnload.Raise(this);
                return;
            }
#endif

            switch (sceneProvider)
            {
                case SceneProvider.BuildIndex:
                    await SceneManager.UnloadSceneAsync(buildIndex);
                    break;
                case SceneProvider.Addressable:
                    if (sceneReference.IsValid())
                    {
                        await sceneReference.UnLoadScene();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            LoadedScenes.Remove(this);
            State = SceneState.Unloaded;
            afterUnload.Raise(this);
        }

        #endregion


        #region Editor

#if UNITY_EDITOR

        [ReadOnly]
        [NonSerialized]
        [ShowInInspector]
        private bool _isLoadedInEditor;

        public void SetIsLoadedInEditor()
        {
            _isLoadedInEditor = true;
            State = SceneState.Loaded;
        }

        [UnityEditor.Callbacks.OnOpenAssetAttribute]
        public static bool OpenSceneTemplate(int instanceID, int line)
        {
            if (UnityEditor.EditorUtility.InstanceIDToObject(instanceID) is not SceneAsset sceneAsset)
            {
                return false;
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                UnityEditor.AssetDatabase.GetAssetPath(sceneAsset.EditorSceneAsset),
                UnityEditor.SceneManagement.OpenSceneMode.Single);

            return false;
        }

        private async UniTask UnloadEditorScene(SceneAsset sceneAsset)
        {
            var sceneCount = SceneManager.sceneCount;
            var path = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset.EditorSceneAsset);
            for (var sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
            {
                if (SceneManager.GetSceneAt(sceneIndex).path != path)
                {
                    continue;
                }
                await SceneManager.UnloadSceneAsync(path).ToUniTask();
                sceneAsset._isLoadedInEditor = false;
                LoadedScenes.Remove(sceneAsset);
                return;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void TrackEditorScenes()
        {
            LoadedScenes.Clear();
            var assets = new List<SceneAsset>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(SceneAsset)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            foreach (var sceneAsset in assets)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset.EditorSceneAsset);
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var active = SceneManager.GetSceneAt(i);
                    if (active.path == path)
                    {
                        sceneAsset.SetIsLoadedInEditor();
                        LoadedScenes.Add(sceneAsset);
                    }
                }
            }
        }

#endif

        #endregion
    }
}