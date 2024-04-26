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

namespace Baracuda.Mediator.Scenes
{
    /// <summary>
    ///     Asset contains a reference for an active scene as well as multiple subscenes that can be loaded all at once.
    /// </summary>
    public class LevelAsset : RegisteredAsset
    {
        #region Inspector

        [Foldout("Scenes")]
        [SerializeField] private SceneAsset sceneAsset;
        [Space]
        [SerializeField] private SceneAsset[] subscenesAssets;

        #endregion


        #region Events & Properties

        /// <summary>
        ///     Currently active and loaded level asset.
        /// </summary>
        [Line]
        [ShowInInspector]
        [ReadOnly]
        public static LevelAsset LoadedLevel { get; private set; }

        /// <summary>
        ///     Called when the loading process of a LevelAsset has started.
        /// </summary>
        public static event Action<LevelAsset> BeforeLoad
        {
            add => beforeLoad.Add(value);
            remove => beforeLoad.Remove(value);
        }

        /// <summary>
        ///     Called when the loading process of a LevelAsset has completed.
        /// </summary>
        public static event Action<LevelAsset> AfterLoad
        {
            add => afterLoad.Add(value);
            remove => afterLoad.Remove(value);
        }

        private static readonly Broadcast<LevelAsset> beforeLoad = new();
        private static readonly Broadcast<LevelAsset> afterLoad = new();

        public SceneAsset MainScene => sceneAsset;
        public IReadOnlyList<SceneAsset> Subscenes => subscenesAssets;

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

            await sceneAsset.LoadAsync(LoadSceneMode.Single);
            LoadedLevel = this;

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
            LoadedLevel = ActiveEditorLayoutAsset;
        }

        public static LevelAsset ActiveEditorLayoutAsset
        {
            get
            {
                var guid = UnityEditor.EditorPrefs.GetString(nameof(LevelAsset));
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<LevelAsset>(path);
            }
            private set
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(value);
                var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(path);
                UnityEditor.EditorPrefs.SetString(nameof(LevelAsset), guid.ToString());
            }
        }

        [PropertySpace]
        [Foldout("Scenes")]
        [Button]
        private void SaveSceneLayout()
        {
            var sceneAssets = GetSceneAssets();

            var mainScene = SceneManager.GetActiveScene();
            if (!TryGetSceneAsset(mainScene, out sceneAsset))
            {
                var instance = CreateInstance<SceneAsset>();
                var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(mainScene.path);
                instance.SetFieldValue("sceneReference", new AssetReferenceScene(guid.ToString()));
                UnityEditor.AssetDatabase.CreateAsset(instance, mainScene.path.Replace(".unity", ".asset"));
                sceneAsset = instance;
            }

            var subsceneBuffer = new List<SceneAsset>();
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
                    asset = CreateInstance<SceneAsset>();
                    var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(scene.path);
                    asset.SetFieldValue("sceneReference", new AssetReferenceScene(guid.ToString()));
                    UnityEditor.AssetDatabase.CreateAsset(asset, scene.path.Replace(".unity", ".asset"));
                    subsceneBuffer.Add(asset);
                }
            }

            subscenesAssets = subsceneBuffer.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
            return;

            bool TryGetSceneAsset(Scene scene, out SceneAsset result)
            {
                foreach (var asset in sceneAssets)
                {
                    var scenePath = UnityEditor.AssetDatabase.GetAssetPath(asset.EditorSceneAsset);
                    if (scenePath == scene.path)
                    {
                        result = asset;
                        return true;
                    }
                }
                result = default(SceneAsset);
                return false;
            }

            static List<SceneAsset> GetSceneAssets()
            {
                var assets = new List<SceneAsset>();
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(SceneAsset)}");
                foreach (var guid in guids)
                {
                    var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
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
            if (UnityEditor.EditorUtility.InstanceIDToObject(instanceID) is not LevelAsset levelAsset)
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
                UnityEditor.AssetDatabase.GetAssetPath(sceneAsset.EditorSceneAsset),
                UnityEditor.SceneManagement.OpenSceneMode.Single);

            foreach (var subsceneAsset in subscenesAssets)
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                    UnityEditor.AssetDatabase.GetAssetPath(subsceneAsset.EditorSceneAsset),
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