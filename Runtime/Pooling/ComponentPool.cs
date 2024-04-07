using Baracuda.Utilities;
using UnityEngine;

namespace Baracuda.Mediator.Pooling
{
    /// <summary>
    ///     Generic <see cref="Component" /> object pool.
    /// </summary>
    public abstract class ComponentPool<TComponent> : PoolAsset<TComponent> where TComponent : Component
    {
        protected override void OnReleaseInstance(TComponent instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(Parent);
        }

        protected override void OnGetInstance(TComponent instance)
        {
            instance.SetActive(true);
        }
    }
}