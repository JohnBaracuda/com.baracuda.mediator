using Baracuda.Tools;
using Sirenix.OdinInspector;

namespace Baracuda.Bedrock.Pooling
{
    public abstract class PoolAsset : MediatorAsset
    {
        /// <summary>
        ///     Preload assets of the pool.
        /// </summary>
        [RuntimeButton]
        [ButtonGroup("Debug/Buttons")]
        public abstract void Load();

        /// <summary>
        ///     Unload assets of the pool. This will force every element of the pool to be returned to the pool.
        /// </summary>
        [RuntimeButton]
        [ButtonGroup("Debug/Buttons")]
        public abstract void Unload();
    }
}