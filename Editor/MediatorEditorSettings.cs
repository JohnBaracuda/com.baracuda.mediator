using Baracuda.Mediator.Generation;
using Baracuda.Utilities.Types;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Mediation.Editor
{
    [UnityEditor.FilePathAttribute("ProjectSettings/MediatorEditorSettings.asset",
        UnityEditor.FilePathAttribute.Location.ProjectFolder)]
    [UnityEditor.InitializeOnLoadAttribute]
    public class MediatorEditorSettings : UnityEditor.ScriptableSingleton<MediatorEditorSettings>
    {
        [Title("Suffix")]
        [SerializeField] private string fallbackSuffix = "Mediator";
        [Space]
        [SerializeField] private Map<MediatorType, string> mediatorTypeSuffix = new()
        {
            {MediatorType.None, "Missing"},
            {MediatorType.ValueAsset, "ValueAsset"},
            {MediatorType.EventAsset, "Event"},
            {MediatorType.PoolAsset, "Pool"},
            {MediatorType.RequestAsset, "Request"},
            {MediatorType.ListAsset, "List"},
            {MediatorType.ArrayAsset, "Array"},
            {MediatorType.HashSetAsset, "HashSet"},
            {MediatorType.SetAsset, "Set"},
            {MediatorType.StackAsset, "Stack"},
            {MediatorType.QueueAsset, "Queue"},
            {MediatorType.DictionaryAsset, "Dictionary"},
            {MediatorType.MapAsset, "Map"},
            {MediatorType.ValueAssetConstant, "SerializedAsset"},
            {MediatorType.ValueAssetRuntime, "RuntimeAsset"},
            {MediatorType.ValueAssetSave, "SaveAsset"},
            {MediatorType.ValueAssetProperty, "PropertyAsset"},
            {MediatorType.LockAsset, "Locks"}
        };

        [Title("Icons")]
        [SerializeField] private Texture2D fallbackIcon;
        [SerializeField] private Map<MediatorType, Texture2D> mediatorTypeIcons = new();

        public IReadOnlyDictionary<MediatorType, string> MediatorTypeSuffix => mediatorTypeSuffix;
        public string FallbackSuffix => fallbackSuffix;

        public IReadOnlyDictionary<MediatorType, Texture2D> MediatorTypeIcons => mediatorTypeIcons;
        public Texture2D FallbackIcon => fallbackIcon;

        public void SaveSettings()
        {
            Save(true);
        }

        static MediatorEditorSettings()
        {
            UnityEditor.EditorApplication.delayCall += Initialize;
        }

        private static void Initialize()
        {
        }
    }
}