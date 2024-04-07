using Baracuda.Mediator.Events;
using Baracuda.Mediator.Registry;
using Baracuda.Tools;
using Baracuda.Utilities.Reflection;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Baracuda.Mediator.Scenes
{
    /// <summary>
    ///     Asset contains a reference for an active scene as well as multiple subscenes that can be loaded all at once.
    /// </summary>
    [AddressablesGroup("Level Assets")]
    public class AddressableLevelAsset : RegisteredAsset
    {
        #region Inspector

        [FormerlySerializedAs("sceneAsset")]
        [Foldout("Scenes")]
        [SerializeField] private AddressableSceneAsset addressableSceneAsset;
        [Space]
        [SerializeField] private AddressableSceneAsset[] subscenesAssets;

        #endregion


        #region Events & Properties

        /// <summary>
        ///     Currently active and loaded level asset.
        /// </summary>
        [Line]
        [ShowInInspector]
        [ReadOnly]
        public static AddressableLevelAsset LoadedAddressableLevel { get; private set; }

        /// <summary>
        ///     Called when the loading process of a LevelAsset has started.
        /// </summary>
        public static event Action<AddressableLevelAsset> BeforeLoad
        {
            add => beforeLoad.Add(value);
            remove => beforeLoad.Remove(value);
        }

        /// <summary>
        ///     Called when the loading process of a LevelAsset has completed.
        /// </summary>
        public static event Action<AddressableLevelAsset> AfterLoad
        {
            add => afterLoad.Add(value);
            remove => afterLoad.Remove(value);
        }

        private static readonly Broadcast<AddressableLevelAsset> beforeLoad = new();
        private static readonly Broadcast<AddressableLevelAsset> afterLoad = new();

        public AddressableSceneAsset MainAddressableScene => addressableSceneAsset;
        public IReadOnlyList<AddressableSceneAsset> Subscenes => subscenesAssets;

        #endregion


        #region Loading

        public void Load()
        {
            LoadAsync().Forget();
        }

        public async UniTask LoadAsync()
        {
            Debug.Log("Scene", $"Begin Loading {name} Level Setup");

            beforeLoad.Raise(this);

            await addressableSceneAsset.LoadAsync(LoadSceneMode.Single);
            LoadedAddressableLevel = this;

            var processes = ListPool<UniTask>.Get();
            foreach (var subscene in subscenesAssets)
            {
                var loadingOperation = subscene.LoadAsync();
                processes.Add(loadingOperation);
            }

            await UniTask.WhenAll(processes);
            ListPool<UniTask>.Release(processes);

            afterLoad.Raise(this);
            Debug.Log("Scene", $"Successfully Loaded {name} Scene Setup");
        }

        #endregion


        #region Editor

#if UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetupEditorLevel()
        {
            LoadedAddressableLevel = ActiveEditorLayoutAsset;
        }

        public static AddressableLevelAsset ActiveEditorLayoutAsset
        {
            get
            {
                var guid = UnityEditor.EditorPrefs.GetString(nameof(AddressableLevelAsset));
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<AddressableLevelAsset>(path);
            }
            private set
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(value);
                var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(path);
                UnityEditor.EditorPrefs.SetString(nameof(AddressableLevelAsset), guid.ToString());
            }
        }

        [PropertySpace]
        [Foldout("Scenes")]
        [Button]
        private void SaveSceneLayout()
        {
            var sceneAssets = GetSceneAssets();

            var mainScene = SceneManager.GetActiveScene();
            if (!TryGetSceneAsset(mainScene, out addressableSceneAsset))
            {
                var instance = CreateInstance<AddressableSceneAsset>();
                var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(mainScene.path);
                instance.SetFieldValue("sceneReference", new AssetReferenceScene(guid.ToString()));
                UnityEditor.AssetDatabase.CreateAsset(instance, mainScene.path.Replace(".unity", ".asset"));
                addressableSceneAsset = instance;
            }

            var subsceneBuffer = new List<AddressableSceneAsset>();
            for (var i = 1; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (scene.isSubScene)
                {
                    continue;
                }

                if (TryGetSceneAsset(scene, out var asset))
                {
                    subsceneBuffer.Add(asset);
                }
                else
                {
                    asset = CreateInstance<AddressableSceneAsset>();
                    var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(scene.path);
                    asset.SetFieldValue("sceneReference", new AssetReferenceScene(guid.ToString()));
                    UnityEditor.AssetDatabase.CreateAsset(asset, scene.path.Replace(".unity", ".asset"));
                    subsceneBuffer.Add(asset);
                }
            }

            subscenesAssets = subsceneBuffer.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
            return;

            bool TryGetSceneAsset(Scene scene, out AddressableSceneAsset result)
            {
                foreach (var asset in sceneAssets)
                {
                    var scenePath = UnityEditor.AssetDatabase.GetAssetPath(asset.SceneReference.editorAsset);
                    if (scenePath == scene.path)
                    {
                        result = asset;
                        return true;
                    }
                }
                result = default(AddressableSceneAsset);
                return false;
            }

            static List<AddressableSceneAsset> GetSceneAssets()
            {
                var assets = new List<AddressableSceneAsset>();
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(AddressableSceneAsset)}");
                foreach (var guid in guids)
                {
                    var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<AddressableSceneAsset>(assetPath);
                    if (asset != null)
                    {
                        assets.Add(asset);
                    }
                }
                return assets;
            }
        }

        [UnityEditor.Callbacks.OnOpenAssetAttribute]
        public static bool OpenSceneTemplate(int instanceID, int line)
        {
            if (UnityEditor.EditorUtility.InstanceIDToObject(instanceID) is not AddressableLevelAsset levelAsset)
            {
                return false;
            }

            levelAsset.OpenInEditor();

            return false;
        }

        [ExitFoldout]
        [ButtonGroup("Buttons")]
        [DisableIf("@UnityEngine.Application.isPlaying")]
        [Button("Open (Editor)", ButtonSizes.Large)]
        private void OpenInEditor()
        {
            Assert.IsTrue(Application.isPlaying is false);
            if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                UnityEditor.AssetDatabase.GetAssetPath(addressableSceneAsset.SceneReference.editorAsset),
                UnityEditor.SceneManagement.OpenSceneMode.Single);

            foreach (var subsceneAsset in subscenesAssets)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                    UnityEditor.AssetDatabase.GetAssetPath(subsceneAsset.SceneReference.editorAsset),
                    UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }

            ActiveEditorLayoutAsset = this;
        }

        [ExitFoldout]
        [ButtonGroup("Buttons")]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        [Button("Open (Runtime)", ButtonSizes.Large)]
        private void OpenInPlayMode()
        {
            Assert.IsTrue(Application.isPlaying);
            LoadAsync().Forget();
        }
#endif

        #endregion
    }
}