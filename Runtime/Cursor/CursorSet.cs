using Baracuda.Bedrock.Callbacks;
using Baracuda.Bedrock.Injection;
using Baracuda.Bedrock.Odin;
using Baracuda.Utilities.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace Baracuda.Bedrock.Cursor
{
    public class CursorSet : ScriptableAsset
    {
        [SerializeField] [Required] private CursorFile fallback;
        [Space]
        [SerializeField] private Map<CursorType, CursorFile> cursorMappings;

        public CursorFile GetCursor(CursorType type)
        {
            if (cursorMappings.TryGetValue(type, out var file) && file != null)
            {
                return file;
            }

            Debug.LogWarning("Cursor Set",
                $"Unable to find cursor file for {type.name} ({type.GetInstanceID()}) in {name}!");

            return fallback;
        }

        public CursorType GetType(CursorFile file)
        {
            foreach (var (cursorType, cursorFile) in cursorMappings)
            {
                if (file == cursorFile)
                {
                    return cursorType;
                }
            }

            Debug.LogWarning("Cursor Set", $"Unable to find cursor type for {file.name} in {name}!");
            return CursorType.None;
        }

#if UNITY_EDITOR

        [Inject] private readonly CursorManager _cursorManager;

        [Button]
        [Line]
        private void Initialize()
        {
            var assets = ListPool<CursorType>.Get();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(CursorType)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<CursorType>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            foreach (var cursorType in assets)
            {
                if (cursorMappings.ContainsKey(cursorType))
                {
                    continue;
                }
                cursorMappings.Add(cursorType, null);
            }

            ListPool<CursorType>.Release(assets);
        }

        [Button]
        private void Clear()
        {
            cursorMappings.Clear();
        }

        [Button]
        private void Activate()
        {
            if (Application.isPlaying)
            {
                _cursorManager.SwitchActiveCursorSet(this);
            }
        }
#endif
    }
}