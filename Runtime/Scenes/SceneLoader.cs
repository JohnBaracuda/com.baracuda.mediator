using Baracuda.Bedrock.Scenes2;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace Baracuda.Bedrock.Scenes
{
    public readonly struct SceneLoader
    {
        #region Constructor

        public readonly bool IsCreated;
        private readonly List<SceneEntry> _entries;

        private SceneLoader(List<SceneEntry> entries)
        {
            IsCreated = true;
            _entries = entries;
        }

        #endregion


        #region Builder API

        public static SceneLoader Create()
        {
            return new SceneLoader(ListPool<SceneEntry>.Get());
        }

        public SceneEntryBuilder ScheduleScene(SceneBuildIndex buildIndex)
        {
            Assert.IsTrue(IsCreated);
            return new SceneEntryBuilder(this, buildIndex);
        }

        public SceneEntryBuilder ScheduleScene(string sceneName)
        {
            Assert.IsTrue(IsCreated);
            return new SceneEntryBuilder(this, SceneUtilityAPI.GetSceneIndexByName(sceneName));
        }

        public SceneEntryBuilder ScheduleScene(SceneReference scene)
        {
            Assert.IsTrue(IsCreated);
            return new SceneEntryBuilder(this, scene.BuildIndex);
        }

        #endregion


        #region Scene Entry Builder

        private void AddEntry(SceneEntry entry)
        {
            _entries.Add(entry);
        }

        private struct SceneEntry
        {
            public bool LoadAsync;
            public bool LoadParallel;
            public bool IsMainScene;
            public bool ActivateOnLoad;
            public SceneBuildIndex BuildIndex;
        }

        public struct SceneEntryBuilder
        {
            private readonly SceneLoader _sceneLoader;
            private SceneEntry _entry;

            public SceneEntryBuilder(SceneLoader sceneLoader, SceneBuildIndex buildIndex)
            {
                _sceneLoader = sceneLoader;
                _entry = new SceneEntry
                {
                    BuildIndex = buildIndex,
                    LoadAsync = false,
                    IsMainScene = false,
                    LoadParallel = false,
                    ActivateOnLoad = true
                };
            }

            public SceneEntryBuilder AsMain()
            {
                _entry.IsMainScene = true;
                return this;
            }

            public SceneEntryBuilder AsAsync()
            {
                _entry.LoadAsync = true;
                return this;
            }

            public SceneEntryBuilder AsParallel()
            {
                _entry.LoadParallel = true;
                return this;
            }

            public SceneEntryBuilder ScheduleScene(SceneBuildIndex buildIndex)
            {
                _sceneLoader.AddEntry(_entry);
                return new SceneEntryBuilder(_sceneLoader, buildIndex);
            }

            public SceneEntryBuilder ScheduleScene(string sceneName)
            {
                _sceneLoader.AddEntry(_entry);
                return new SceneEntryBuilder(_sceneLoader, SceneUtilityAPI.GetSceneIndexByName(sceneName));
            }

            public SceneEntryBuilder ScheduleScene(SceneReference scene)
            {
                _sceneLoader.AddEntry(_entry);
                return new SceneEntryBuilder(_sceneLoader, scene.BuildIndex);
            }

            public SceneLoader Build()
            {
                _sceneLoader.AddEntry(_entry);
                return _sceneLoader;
            }

            public UniTask LoadAsync()
            {
                return Build().LoadAsync();
            }
        }

        #endregion


        #region Loading

        public async UniTask LoadAsync()
        {
            Assert.IsTrue(IsCreated);

            var loadSceneMode = LoadSceneMode.Single;
            foreach (var sceneEntry in _entries)
            {
                var buildIndex = sceneEntry.BuildIndex;

                if (sceneEntry.LoadAsync)
                {
                    var loadOperation = SceneManager.LoadSceneAsync(buildIndex, loadSceneMode);
                    loadOperation!.allowSceneActivation = sceneEntry.ActivateOnLoad;
                    await loadOperation.ToUniTask();
                }
                else
                {
                    SceneManager.LoadScene(buildIndex, loadSceneMode);
                }

                if (sceneEntry.IsMainScene)
                {
                    var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
                    SceneManager.SetActiveScene(scene);
                }

                loadSceneMode = LoadSceneMode.Additive;
            }

            ListPool<SceneEntry>.Release(_entries);
        }

        #endregion
    }
}