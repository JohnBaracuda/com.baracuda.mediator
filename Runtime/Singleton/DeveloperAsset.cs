using Baracuda.Bedrock.Callbacks;
using Baracuda.Bedrock.Registry;
using Baracuda.Tools;
using Sirenix.OdinInspector;
using System;

namespace Baracuda.Bedrock.Singleton
{
    /// <summary>
    ///     Base type for developer specific configuration files.
    /// </summary>
    public abstract class DeveloperAsset<T> : ScriptableAsset where T : DeveloperAsset<T>
    {
        private static T local;

        /// <summary>
        ///     The locally saved/applied configuration file.
        /// </summary>
        public static T Singleton
        {
            get
            {
#if !UNITY_EDITOR
                return AssetRegistry.ResolveSingleton<T>();
#else
                if (local == null)
                {
                    var guid = UnityEditor.EditorPrefs.GetString(typeof(T).FullName);
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);

                    Singleton = asset;
                }

                if (local == null)
                {
                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Settings");
                    }

                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings/Developer"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets/Settings", "Developer");
                    }

                    var asset = CreateInstance<T>();
                    var assetPath = $"Assets/Settings/Developer/{typeof(T).Name}.{Environment.UserName.Trim()}.asset";
                    UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                    UnityEditor.AssetDatabase.SaveAssets();
                    Debug.Log("Singleton", $"Creating new {typeof(T).Name} instance at {assetPath}!");
                    Singleton = asset;
                }

                if (local == null)
                {
                    Singleton = AssetRegistry.ResolveSingleton<T>();
                }

                return local;
#endif
            }
            private set
            {
#if UNITY_EDITOR
                if (value == null)
                {
                    return;
                }

                var path = UnityEditor.AssetDatabase.GetAssetPath(value);
                var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                UnityEditor.EditorPrefs.SetString(typeof(T).FullName, guid);
                if (AssetRegistry.ExistsSingleton<T>() is false)
                {
                    AssetRegistry.RegisterSingleton(value);
                }
#endif
                local = value;
            }
        }

#if UNITY_EDITOR

        private bool IsLocal()
        {
            return this == Singleton;
        }

        private bool IsGlobal()
        {
            return this == AssetRegistry.ResolveSingleton<T>();
        }

        [Button]
        [Foldout("Asset")]
        [HideIf(nameof(IsLocal))]
        public void DeclareAsLocal()
        {
            Singleton = (T) this;
        }

        [Button]
        [Foldout("Asset")]
        [HideIf(nameof(IsGlobal))]
        public void DeclareAsGlobal()
        {
            AssetRegistry.RegisterSingleton((T) this);
        }

#endif
    }
}