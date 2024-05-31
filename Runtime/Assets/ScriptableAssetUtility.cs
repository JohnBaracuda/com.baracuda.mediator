using Baracuda.Bedrock.Callbacks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.Utility
{
    public static class ScriptableAssetUtility
    {
        private static readonly Dictionary<Type, string> templateCache = new();

        /// <summary>
        ///     Get the json representation for this asset.
        /// </summary>
        public static string GetAssetJSon(ScriptableObject scriptableObject)
        {
            return JsonUtility.ToJson(scriptableObject);
        }

        /// <summary>
        ///     Set the json representation for this asset.
        /// </summary>
        public static void SetAssetJSon(ScriptableObject scriptableObject, string json)
        {
            JsonUtility.FromJsonOverwrite(json, scriptableObject);
        }

        /// <summary>
        ///     Reset the passed scriptableObject to its default values
        /// </summary>
        public static void ResetAsset(ScriptableObject scriptableObject, bool cache = true)
        {
            var type = scriptableObject.GetType();

            if (!templateCache.TryGetValue(type, out var json))
            {
                var template = ScriptableObject.CreateInstance(type);
                json = JsonUtility.ToJson(template);
                Gameloop.Unregister(template);
                Object.DestroyImmediate(template);
            }
            if (cache)
            {
                templateCache.TryAdd(type, json);
            }

            JsonUtility.FromJsonOverwrite(json, scriptableObject);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(scriptableObject);
#endif
        }
    }
}