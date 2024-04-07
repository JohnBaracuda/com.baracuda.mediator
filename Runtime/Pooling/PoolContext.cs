using UnityEngine;

namespace Baracuda.Mediator.Pooling
{
    /// <summary>
    ///     Asset represents a collection of pools that can be loaded and unloaded at once.
    /// </summary>
    public class PoolContext : ScriptableObject
    {
        [SerializeField] private PoolAsset[] pools;

        public void Load()
        {
            foreach (var poolAsset in pools)
            {
                poolAsset.Load();
            }
        }

        public void Unload()
        {
            foreach (var poolAsset in pools)
            {
                poolAsset.Unload();
            }
        }
    }
}