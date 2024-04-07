using UnityEngine;

namespace Baracuda.Mediator.Pooling
{
    /// <summary>
    ///     Generic <see cref="GameObject" /> object pool.
    /// </summary>
    public class GameObjectPool : PoolAsset<GameObject>
    {
        protected sealed override void OnReleaseInstance(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(Parent);
        }

        protected sealed override void OnGetInstance(GameObject instance)
        {
            instance.SetActive(true);
        }
    }
}