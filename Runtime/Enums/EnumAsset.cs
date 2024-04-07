using Baracuda.Mediator.Callbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Mediator.Enums
{
    public abstract class EnumAsset<T> : ScriptableAsset where T : EnumAsset<T>
    {
        [ReadOnly] [SerializeField] private int hash;

        public static T None => none ??= CreateInstance<T>();

        private static T none;
    }
}