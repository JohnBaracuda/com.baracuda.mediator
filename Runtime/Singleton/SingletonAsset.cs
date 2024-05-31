using Baracuda.Bedrock.Callbacks;
using Baracuda.Bedrock.Registry;
using JetBrains.Annotations;

namespace Baracuda.Bedrock.Singleton
{
    public abstract class SingletonAsset<T> : ScriptableAsset where T : SingletonAsset<T>
    {
        public static T Singleton => singleton ??= AssetRegistry.ResolveSingleton<T>();
        private static T singleton;

        [PublicAPI]
        public bool IsSingleton => AssetRegistry.ExistsSingleton<T>() && AssetRegistry.ResolveSingleton<T>() == this;

        protected override void OnEnable()
        {
            base.OnEnable();
            singleton = (T) this;

            if (AssetRegistry.ExistsSingleton<T>() is false)
            {
                AssetRegistry.RegisterSingleton(this);
            }
        }
    }
}