using Baracuda.Bedrock.Assets;
using Baracuda.Bedrock.Odin;
using Baracuda.Bedrock.PlayerLoop;
using Baracuda.Bedrock.Scenes;
using Baracuda.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Baracuda.Bedrock.Initialization
{
    public class Bootstrap : MonoBehaviour
    {
        #region Fields

        [SerializeField] private SceneReference firstLevel;

        [Header("File System")]
        [Preserve]
        [InlineInspector]
        [SerializeField] private FileSystemArgumentsAsset fileSystemArgumentsRelease;
        [Preserve]
        [InlineInspector]
        [SerializeField] private FileSystemArgumentsAsset fileSystemArgumentsDebug;
        [Preserve]
        [InlineInspector]
        [SerializeField] private FileSystemArgumentsAsset fileSystemArgumentsEditor;

        [SerializeField] private string bootstrapKey = "Preload";

        #endregion


        #region Initialization

        private void Awake()
        {
            Initialize().Forget();
        }

        private async UniTaskVoid Initialize()
        {
            Debug.Log(nameof(Bootstrap), "Initialization Started");

#if UNITY_EDITOR
            await Resources.UnloadUnusedAssets();
#endif

            Debug.Log(nameof(Bootstrap), "(1/5) Initializing File System");
#if UNITY_EDITOR
            if (FileSystem.IsInitialized)
            {
                var args = new FileSystemShutdownArgs
                {
                    forceSynchronousShutdown = true
                };

                await FileSystem.ShutdownAsync(args);
            }
#endif

            var arguments =
#if UNITY_EDITOR
                fileSystemArgumentsEditor;
#elif DEBUG
                fileSystemArgumentsDebug;
#else
                fileSystemArgumentsEditor;
#endif

            await FileSystem.InitializeAsync(arguments);

            Debug.Log(nameof(Bootstrap), "(2/5) Loading Addressables");

            await Addressables.InitializeAsync();

            var locations = await Addressables.LoadResourceLocationsAsync(bootstrapKey);

            foreach (var resourceLocation in locations)
            {
                Debug.Log(nameof(Bootstrap),
                    $"Loading Resource [{resourceLocation.PrimaryKey}]");

                await Addressables.LoadAssetAsync<Object>(resourceLocation);
            }

            Debug.Log(nameof(Bootstrap), "(3/5) Loading Resources");

            await Resources.LoadAsync<AssetRepository>("AssetRepository");

            Debug.Log(nameof(Bootstrap), "(4/5) Installing Runtime Systems");
            AssetRepository.Singleton.InstallRuntimeSystems();

            Gameloop.RaiseInitializationCompleted();

            Debug.Log(nameof(Bootstrap), "(5/5) Loading First Level");

            await SceneLoader
                .Create()
                .ScheduleScene(firstLevel)
                .AsBlocking()
                .LoadAsync();

            Debug.Log(nameof(Bootstrap), "Initialization Completed");
        }

        #endregion


        #region Debug & Editor

#if UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            var activeScene = SceneManager.GetActiveScene();
            var sceneIndex = SceneUtility.GetBuildIndexByScenePath(activeScene.path);
            var isInitializationScene = sceneIndex == 0;

            if (isInitializationScene)
            {
                return;
            }

            Debug.Log("Bootstrap", "Initializing Editor Systems");

            if (FileSystem.IsInitialized is false)
            {
                Debug.LogError("Please make sure to launch the game with an initialized File System int the Editor!");
            }

            Debug.Log("Bootstrap", "Installing Runtime Systems");
            var assetRegistry = Resources.Load<AssetRepository>("AssetRepository");
            assetRegistry.InstallRuntimeSystems();
            Gameloop.RaiseInitializationCompleted();
        }
#endif

        #endregion
    }
}