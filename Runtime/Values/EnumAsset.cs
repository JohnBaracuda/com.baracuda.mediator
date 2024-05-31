using UnityEngine;

namespace Baracuda.Bedrock.Values
{
    public abstract class EnumAsset<T> : ScriptableObject where T : EnumAsset<T>
    {
        public static T None => none ??= CreateInstance<T>();

        private static T none;
    }
}