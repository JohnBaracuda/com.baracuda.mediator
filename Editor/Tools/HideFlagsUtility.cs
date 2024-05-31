using Baracuda.Tools;
using System.Reflection;
using UnityEngine;

namespace Baracuda.Bedrock.Editor.Tools
{
    public static class HideFlagsUtility
    {
        public static void ShowAllHiddenObjects()
        {
            var allGameObjects =
                Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var gameObject in allGameObjects)
            {
                switch (gameObject.hideFlags)
                {
                    case HideFlags.HideAndDontSave:
                        gameObject.hideFlags = HideFlags.DontSave;
                        break;
                    case HideFlags.HideInHierarchy:
                    case HideFlags.HideInInspector:
                        gameObject.hideFlags = HideFlags.None;
                        break;
                }
            }

            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }

        public static void ShowAllHiddenInspector()
        {
            var allGameObjects =
                Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var gameObject in allGameObjects)
            {
                foreach (var component in gameObject.GetComponents<Component>())
                {
                    switch (component.hideFlags)
                    {
                        case HideFlags.HideAndDontSave:
                            component.hideFlags = HideFlags.DontSave;
                            break;
                        case HideFlags.HideInHierarchy:
                        case HideFlags.HideInInspector:
                            component.hideFlags = HideFlags.None;
                            break;
                    }
                }
            }

            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }

        [UnityEditor.InitializeOnLoadMethodAttribute]
        public static void ValidateAllObjectsHideFlags()
        {
            var monoBehaviours =
                Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour.GetType().GetCustomAttribute<HideFlagsAttribute>() is { } attribute)
                {
                    var gameObject = monoBehaviour.gameObject;

                    if (attribute.InternalObjectFlags.HasValue)
                    {
                        gameObject.hideFlags = attribute.ObjectFlags;
                        UnityEditor.EditorUtility.SetDirty(gameObject);
                    }

                    if (attribute.InternalScriptFlags.HasValue)
                    {
                        monoBehaviour.hideFlags = attribute.ScriptFlags;
                        UnityEditor.EditorUtility.SetDirty(monoBehaviour);
                    }
                }
            }

            UnityEditor.EditorApplication.DirtyHierarchyWindowSorting();
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }

        [UnityEditor.MenuItem("GameObject/Hide GameObject")]
        public static void HideSelectedGameObject(UnityEditor.MenuCommand command)
        {
            if (command.context != null)
            {
                command.context.hideFlags |= HideFlags.HideInHierarchy;
            }
        }
    }
}