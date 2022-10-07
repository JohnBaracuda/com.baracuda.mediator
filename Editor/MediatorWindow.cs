using Baracuda.EditorUtilities.Helper;
using Baracuda.EditorUtilities.Inspector;
using Baracuda.Mediator.Events;
using Baracuda.Mediator.ValueObjects;
using UnityEditor;

namespace Baracuda.Mediator.Editor
{
    public class MediatorWindow : AssetCollectionWindow
    {
        [MenuItem("Framework/Mediator")]
        private static void OpenWindow()
        {
            GetWindow<MediatorWindow>("Mediator Window");
        }

        protected override void Initialize()
        {
            AddAssetCollection("Events", EditorHelper.FindAssetsOfType<EventChannelBase>(), "No Events");
            AddAssetCollection("Value Assets", EditorHelper.FindAssetsOfType<ValueAsset>(), "No Mediator Objects");
            SetDefaultFilterString("Mediator");
        }
    }
}
