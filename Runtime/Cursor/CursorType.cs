using Baracuda.Mediator.Enums;
using Baracuda.Mediator.Registry;
using Baracuda.Utilities.Reflection;
using Sirenix.OdinInspector;

namespace Baracuda.Mediator.Cursor
{
    [AddressablesGroup("Cursor")]
    public class CursorType : EnumAsset<CursorType>
    {
        [ReadOnly]
        public int SingletonInstanceId { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            SingletonInstanceId = AssetRegistry.Singleton.GetInstanceID();
        }
    }
}