using Baracuda.Mediator.Callbacks;
using Baracuda.Mediator.Events;
using Baracuda.Mediator.Registry;
using Baracuda.Utilities.Reflection;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Baracuda.Mediator.Scenes
{
    [AddressablesGroup("Scene Assets")]
    public class AddressableSceneAsset : RegisteredAsset
    {
        #region Instance

        [SerializeField] [Required] private AssetReferenceScene sceneReference;

        [ShowInInspector] [ReadOnly]
        public SceneState State { get; internal set; }

        public AssetReferenceScene SceneReference => sceneReference;

        #endregion


        #region Static

        /// <summary>
        ///     Set of all currently loaded scene assets.
        /// </summary>
        [ShowInInspector] [ReadOnly]
        public static HashSet<AddressableSceneAsset> LoadedScenes { get; } = new();

        public static event Action<AddressableSceneAsset> BeforeLoad
        {
            add => beforeLoad.Add(value);
            remove => beforeLoad.Remove(value);
        }

        public static event Action<AddressableSceneAsset> AfterLoad
        {
            add => afterLoad.Add(value);
            remove => afterLoad.Remove(value);
        }

        public static event Action<AddressableSceneAsset> BeforeUnload
        {
            add => beforeUnload.Add(value);
            remove => beforeUnload.Remove(value);
        }

        public static event Action<AddressableSceneAsset> AfterUnload
        {
            add => afterUnload.Add(value);
            remove => afterUnload.Remove(value);
        }

        #endregion


        #region Fields

        private static readonly Broadcast<AddressableSceneAsset> beforeLoad = new();
        private static readonly Broadcast<AddressableSceneAsset> afterLoad = new();
        private static readonly Broadcast<AddressableSceneAsset> beforeUnload = new();
        private static readonly Broadcast<AddressableSceneAsset> afterUnload = new();

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
            if (SceneReference.editorAsset == null)
            {
                Debug.LogError("Scene Reference is not set!", this);
                return;
            }
#endif

            if (State is not (SceneState.None or SceneState.Unloaded))
            {
                Debug.LogWarning("Scene", $"Cannot load {this} because it is {State}");
                return;
            }

            Debug.Log("Scene", $"Loading {this}");

            State = SceneState.Loading;
            beforeLoad.Raise(this);

            await sceneReference.LoadSceneAsync(LoadSceneMode.Additive);

            LoadedScenes.Add(this);
            State = SceneState.Loaded;
            afterLoad.Raise(this);
        }

        private async UniTask LoadSingleAsyncInternal()
        {
#if UNITY_EDITOR
            if (SceneReference.editorAsset == null)
            {
                Debug.LogError("Scene Reference is not set!", this);
                return;
            }
#endif

            if (State is not (SceneState.None or SceneState.Unloaded))
            {
                Debug.LogWarning("Scene", $"Cannot load {this} because it is {State}");
                return;
            }

            Debug.Log("Scene", $"Loading {this}");

            var scenes = LoadedScenes.ToArray();
            foreach (var activeScene in scenes)
            {
                activeScene.State = SceneState.Unloading;
            }

            State = SceneState.Loading;
            beforeLoad.Raise(this);

            await sceneReference.LoadSceneAsync();

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
            if (SceneReference.editorAsset == null)
            {
                Debug.LogError("Scene Reference is not set!", this);
                return;
            }
#endif

            if (State is not SceneState.Loaded)
            {
                Debug.LogWarning("Scene", $"Cannot unload {this} because it is {State}");
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
            await UnloadSceneAsync(SceneReference);
            LoadedScenes.Remove(this);
            State = SceneState.Unloaded;
            afterUnload.Raise(this);
            return;

            async UniTask<SceneInstance> UnloadSceneAsync(AssetReferenceScene scene)
            {
                if (!scene.IsValid())
                {
                    return default(SceneInstance);
                }

                var sceneInstance = await scene.UnLoadScene();
                return sceneInstance;
            }
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
            if (UnityEditor.EditorUtility.InstanceIDToObject(instanceID) is not AddressableSceneAsset sceneAsset)
            {
                return false;
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                UnityEditor.AssetDatabase.GetAssetPath(sceneAsset.SceneReference.editorAsset),
                UnityEditor.SceneManagement.OpenSceneMode.Single);

            return false;
        }

        private async UniTask UnloadEditorScene(AddressableSceneAsset addressableSceneAsset)
        {
            var sceneCount = SceneManager.sceneCount;
            var path = UnityEditor.AssetDatabase.GetAssetPath(addressableSceneAsset.SceneReference.editorAsset);
            for (var sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
            {
                if (SceneManager.GetSceneAt(sceneIndex).path != path)
                {
                    continue;
                }
                await SceneManager.UnloadSceneAsync(path).ToUniTask();
                addressableSceneAsset._isLoadedInEditor = false;
                LoadedScenes.Remove(addressableSceneAsset);
                return;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void TrackEditorScenes()
        {
            LoadedScenes.Clear();
            var assets = new List<AddressableSceneAsset>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(AddressableSceneAsset)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<AddressableSceneAsset>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            foreach (var sceneAsset in assets)
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset.SceneReference.editorAsset);
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