using Baracuda.Bedrock.Assets;
using JetBrains.Annotations;

namespace Baracuda.Bedrock.Singleton
{
    public abstract class SingletonAsset<T> : ScriptableAsset where T : SingletonAsset<T>
    {
        public static T Singleton => singleton ??= AssetRepository.ResolveSingleton<T>();
        private static T singleton;

        [PublicAPI]
        public bool IsSingleton =>
            AssetRepository.ExistsSingleton<T>() && AssetRepository.ResolveSingleton<T>() == this;

        protected override void OnEnable()
        {
            base.OnEnable();
            singleton = (T)this;

            if (AssetRepository.ExistsSingleton<T>() is false)
            {
                AssetRepository.RegisterSingleton(this);
            }
        }
    }
}