using UnityEngine.SceneManagement;

namespace Baracuda.Bedrock.Scenes2
{
    public static class SceneUtilityAPI
    {
        public static string GetSceneNameByIndex(int buildIndex)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            var slash = path.LastIndexOf('/');
            var name = path[(slash + 1)..];
            var dot = name.LastIndexOf('.');
            return name[..dot];
        }

        public static int GetSceneIndexByName(string name)
        {
            for (var index = 0; index < SceneManager.sceneCountInBuildSettings; index++)
            {
                if (GetSceneNameByIndex(index) == name)
                {
                    return index;
                }
            }
            Debug.LogError(name);
            return -1;
        }

        public static string GetSceneNameByPath(string path)
        {
            var buildIndex = SceneUtility.GetBuildIndexByScenePath(path);
            var name = GetSceneNameByIndex(buildIndex);
            return name;
        }

        public static bool IsSceneLoaded(int sceneIndex)
        {
            for (var loadedSceneIndex = 0; loadedSceneIndex < SceneManager.loadedSceneCount; loadedSceneIndex++)
            {
                if (SceneManager.GetSceneAt(loadedSceneIndex).buildIndex == sceneIndex)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSceneLoadedAndActive(int sceneIndex)
        {
            return SceneManager.GetActiveScene().buildIndex == sceneIndex;
        }
    }
}